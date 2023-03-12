using BigBrother.Extensions;
using BigBrother.Interfaces;
using Entities.Enums;
using Entities.Exceptions;

namespace BigBrother.Services;

public class GenerationFromScratchDetectionService: IGenerationFromScratchDetectionService
{
    private const int MaxActionsCount = Int32.MaxValue;
    private const double NormalizationValue = 0.16;

    private readonly IUserService _userService;
    private readonly IExamService _examService;
    private readonly ILogger<GenerationFromScratchDetectionService> _logger;

    public GenerationFromScratchDetectionService(
        IUserService userService, 
        IExamService examService,
        ILogger<GenerationFromScratchDetectionService> logger)
    {
        _userService = userService;
        _examService = examService;
        _logger = logger;
    }
    
    public async Task<IDictionary<Guid, double>> DetectGenerationFromScratchAsync(string group, DateTime dateTime, CancellationToken cancellationToken)
    {
        var outlierScores = new Dictionary<Guid, double>() as IDictionary<Guid, double>;
        
        var usersForDetection = _userService.GetUsersFromGroup(group);
        var exams = usersForDetection.Select(user => _examService.GetUserExamAtDate(user, dateTime)).ToList();
        
        if (exams.Count < 2)
        {
            throw new BbException(ErrorCode.TOO_FEW_EXAMS, $"Exams count: {exams.Count}");
        }

        var committedActionsForExams = exams
            .Select(x => x.GetCommittedActions())
            .ToArray();
        var boxCoxDistributions = committedActionsForExams
            .Select(x => x.TransformToBoxCoxDistribution())
            .ToArray();

        var meansFromBoxCoxDistributions = await GetMeansFromBoxCoxDistributionsAsync(boxCoxDistributions, cancellationToken);
        var standardDeviationsFromBoxCoxDistributions = await GetStandardDeviationsFromBoxCoxDistributionAsync(boxCoxDistributions, meansFromBoxCoxDistributions, cancellationToken);
        var actionWeights = await GetActionWeightsAsync(committedActionsForExams, cancellationToken);

        foreach (var exam in exams)
        {
            var examOutlierScore = await GetOutlierScoreForExamAsync(
                exam.GetCommittedActions(), 
                meansFromBoxCoxDistributions,
                standardDeviationsFromBoxCoxDistributions, 
                actionWeights,
                cancellationToken);
            
            outlierScores.Add(exam.Id, examOutlierScore);
            _logger.LogInformation($"Outlier score for exam {exam.Id} is: {examOutlierScore}");
        }

        return outlierScores;
    }
    
    private async Task<IDictionary<UserAction, double>> GetMeansFromBoxCoxDistributionsAsync(
        IReadOnlyCollection<IDictionary<UserAction, double>> boxCoxExamDistributions, 
        CancellationToken cancellationToken)
    {
        var meansFromBoxCoxDistributions = new Dictionary<UserAction, double>();
        
        await Task.Run(() =>
        {
            double examCount = boxCoxExamDistributions.Count;

            foreach (var boxCoxDistribution in boxCoxExamDistributions)
            {
                foreach (var userAction in boxCoxDistribution.Keys)
                {
                    if (!meansFromBoxCoxDistributions.ContainsKey(userAction))
                    {
                        meansFromBoxCoxDistributions[userAction] = 0;
                    }
                
                    meansFromBoxCoxDistributions[userAction] += boxCoxDistribution[userAction] / examCount;
                
                }
            }
        }, cancellationToken);

        return meansFromBoxCoxDistributions;
    }

    private async Task<IDictionary<UserAction, double>> GetStandardDeviationsFromBoxCoxDistributionAsync(
        IReadOnlyCollection<IDictionary<UserAction, double>> boxCoxDistributions,
        IDictionary<UserAction, double> meansFromBoxCoxDistributions,
        CancellationToken cancellationToken)
    {
        var standardDeviationsFromBoxCoxDistributions = new Dictionary<UserAction, double>();
        double examCount = boxCoxDistributions.Count;

        await Task.Run(() =>
        {
            foreach (var boxCoxDistribution in boxCoxDistributions)
            {
                foreach (var userAction in boxCoxDistribution.Keys)
                {
                    if (!standardDeviationsFromBoxCoxDistributions.ContainsKey(userAction))
                    {
                        standardDeviationsFromBoxCoxDistributions[userAction] = 0;
                    }

                    standardDeviationsFromBoxCoxDistributions[userAction] =
                        Math.Pow(boxCoxDistribution[userAction] - meansFromBoxCoxDistributions[userAction], 2);
                }
            }
            
            standardDeviationsFromBoxCoxDistributions = standardDeviationsFromBoxCoxDistributions
                .Select(x => new KeyValuePair<UserAction, double>(x.Key, Math.Sqrt(x.Value / examCount)))
                .ToDictionary(x => x.Key, x => x.Value);
        }, cancellationToken);

        return standardDeviationsFromBoxCoxDistributions;
    }
    
    private async Task<IDictionary<UserAction, double>> GetActionWeightsAsync(
        IReadOnlyCollection<IDictionary<UserAction, int>> committedActionsInExams, 
        CancellationToken cancellationToken)
    {
        var actionsWeights = new Dictionary<UserAction, double>();
        var examCount = committedActionsInExams.Count;

        await Task.Run(() =>
        {
            foreach (var committedActions in committedActionsInExams)
            {
                foreach (var userAction in committedActions.Keys)
                {
                    if (!actionsWeights.ContainsKey(userAction))
                    {
                        actionsWeights[userAction] = 0;
                    }

                    actionsWeights[userAction] += committedActions[userAction] > 0 ? 1 : 0;
                }
            }
            
            actionsWeights = actionsWeights
                .Select(x => new KeyValuePair<UserAction, double>(x.Key, Math.Pow(x.Value / examCount, 2)))
                .ToDictionary(x => x.Key, x => x.Value);
        }, cancellationToken);

        return actionsWeights;
    }
    
    private async Task<double> GetOutlierScoreForExamAsync(
        IDictionary<UserAction, int> committedActionsInExam,
        IDictionary<UserAction, double> meansFromBoxCoxDistributions,
        IDictionary<UserAction, double> standardDeviationsFromBoxCoxDistributions,
        IDictionary<UserAction, double> actionWeights,
        CancellationToken cancellationToken)
    {
        double numerator = 0;
        double denominator = 0;

        var boxCoxDistribution = committedActionsInExam.TransformToBoxCoxDistribution();

        var unweightedOutlierScore = await GetUnweightedOutlierScoreForActionsInExamAsync(
            boxCoxDistribution, 
            meansFromBoxCoxDistributions, 
            standardDeviationsFromBoxCoxDistributions, 
            cancellationToken);
        
        foreach (var userAction in committedActionsInExam.Keys)
        {
            numerator += actionWeights[userAction] * unweightedOutlierScore[userAction];
            denominator += actionWeights[userAction];
        }

        return numerator / denominator;
    }

    private async Task<IDictionary<UserAction, double>> GetUnweightedOutlierScoreForActionsInExamAsync(
        IDictionary<UserAction, double> boxCoxDistribution,
        IDictionary<UserAction, double> meansDistribution,
        IDictionary<UserAction, double> standardDeviationDistribution, 
        CancellationToken cancellationToken)
    {
        var unweightedOutlierScore = new Dictionary<UserAction, double>();

        await Task.Run(() =>
        {
            foreach (var userAction in boxCoxDistribution.Keys)
            {
                var difference = boxCoxDistribution[userAction] - meansDistribution[userAction];
                var standardDeviation = standardDeviationDistribution[userAction];
                var outlierScore = -standardDeviation <= difference && difference <= standardDeviation
                    ? 0
                    : (1 - GetPraw(boxCoxDistribution[userAction], userAction, meansDistribution)) / NormalizationValue;

                unweightedOutlierScore.Add(userAction, outlierScore);
            }
        }, cancellationToken);

        return unweightedOutlierScore;
    }
    
    private double GetPraw(double boxCoxActionValue, UserAction userAction, IDictionary<UserAction, double> meansDistributions)
    {
        return boxCoxActionValue > meansDistributions[userAction]
            ? boxCoxActionValue / MaxActionsCount
            : 1 - boxCoxActionValue / MaxActionsCount;
    }
}