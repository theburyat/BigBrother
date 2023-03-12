using BigBrother.Extensions;
using BigBrother.Interfaces;
using Entities.Domain;
using Entities.Enums;
using Entities.Exceptions;

namespace BigBrother.Services;

public class CopyAndModifyDetectionService: ICopyAndModifyDetectionService
{
    private const int SubsetAnalysisInPairCount = 5; 
    
    private readonly Random _random = new ();
    
    private readonly IUserService _userService;
    private readonly IExamService _examService;
    private readonly ILogger<CopyAndModifyDetectionService> _logger;

    public CopyAndModifyDetectionService(
        IUserService userService, 
        IExamService examService, 
        ILogger<CopyAndModifyDetectionService> logger)
    {
        _userService = userService;
        _examService = examService;
        _logger = logger;
    }
    
    public async Task<IDictionary<Tuple<Guid, Guid>, double>> DetectCopyAndModify(string group, DateTime dateTime)
    {
        var usersForDetection = _userService.GetUsersFromGroup(group);

        var exams = usersForDetection.Select(user => _examService.GetUserExamAtDate(user, dateTime)).ToList();

        if (exams.Count < 2)
        {
            throw new BbException(ErrorCode.TOO_FEW_EXAMS, $"Exams count: {exams.Count}");
        }

        var detectionsResults = new Dictionary<Tuple<Guid, Guid>, double>();

        for (var i = 0; i < exams.Count - 1; i++)
        {
            for (var j = i + 1; j < exams.Count; j++)
            {
                var correlation = await DetectCopyAndModifyInPairAsync(exams[i], exams[j]);
                _logger.LogInformation($"Correlation between exams {exams[i].Id} and {exams[j].Id} is: {correlation}");
                
                detectionsResults.Add(new Tuple<Guid, Guid>(exams[i].Id, exams[j].Id), correlation);
            }
        }

        return detectionsResults;
    }

    private async Task<double> DetectCopyAndModifyInPairAsync(Exam exam1, Exam exam2)
    {
        var actionStatistic1 = exam1.GetCommittedActions();
        var actionStatistic2 = exam2.GetCommittedActions();

        var correlations = new List<double>();

        for (var i = 0; i < SubsetAnalysisInPairCount; i++) //TODO() work in parallel
        {
            var correlation = await MakeAnalysisForRandomActionsSubsetAsync(actionStatistic1, actionStatistic2);
            correlations.Add(correlation);
        }

        return correlations.Max();
    }

    private async Task<double> MakeAnalysisForRandomActionsSubsetAsync(
        IDictionary<UserAction, int> actionStatistic1,
        IDictionary<UserAction, int> actionStatistic2)
    {
        double numerator = default;
        double denominator = default;
        
        await Task.Run(() =>
        {
            var actionsSet = GetActionSetForAnalyse(actionStatistic1, actionStatistic2);
            if (actionsSet.Count == 0)
            {
                throw new BbException(ErrorCode.TOO_FEW_ACTIONS, $"To few actions for analysis: {actionsSet.Count}");
            }

            var quotient1 = GetQuotient(actionsSet, actionStatistic1);
            var quotient2 = GetQuotient(actionsSet, actionStatistic2);

            numerator = GetNumerator(actionsSet, actionStatistic1, actionStatistic2, quotient1, quotient2);
            denominator = GetDenominatorPartForExam(actionsSet, actionStatistic1, quotient1) *
                          GetDenominatorPartForExam(actionsSet, actionStatistic2, quotient2);
        });

        return numerator / denominator;
    }

    private ISet<UserAction> GetActionSetForAnalyse(
        IDictionary<UserAction, int> actionStatistic1, 
        IDictionary<UserAction, int> actionStatistic2)
    {
        var statisticWithoutEmptyActions1 = actionStatistic1.Where(x => x.Value > 0).ToDictionary(x => x.Key, y => y.Value);
        var statisticWithoutEmptyActions2 = actionStatistic2.Where(x => x.Value > 0).ToDictionary(x => x.Key, y => y.Value);

        var actionSet1 = GetSetOfHalfOfActions(statisticWithoutEmptyActions1);
        var actionSet2 = GetSetOfHalfOfActions(statisticWithoutEmptyActions2);

        actionSet1.UnionWith(actionSet2);

        return actionSet1;
    }

    private ISet<UserAction> GetSetOfHalfOfActions(IDictionary<UserAction, int> actionStatistic)
    {
        var resultSet = new HashSet<UserAction>();
        
        var actionSet = actionStatistic.Select(x => x.Key).ToHashSet();
        var tempLength = actionSet.Count / 2;

        while (tempLength > 0)
        {
            var randomAction = actionSet.ElementAtOrDefault(_random.Next(0, actionSet.Count));
            if (randomAction is UserAction.None)
            {
                throw new BbException(ErrorCode.INVALID_ACTION, $"Can not get action: {randomAction}");
            }

            resultSet.Add(randomAction);
            actionSet.Remove(randomAction);
            
            tempLength--;
        }

        return resultSet;
    }
    
    private double GetQuotient(ISet<UserAction> actionSet, IDictionary<UserAction, int> actionStatistic)
    {
        double numerator = 0;
        
        foreach (var action in actionSet)
        {
            numerator += actionStatistic[action];
        }

        return numerator / actionSet.Count;
    }
    
    private double GetNumerator(
        ISet<UserAction> actionSet,
        IDictionary<UserAction, int> actionStatistic1,
        IDictionary<UserAction, int> actionStatistic2,
        double quotient1,
        double quotient2)
    {
        double result = 0;

        foreach (var action in actionSet)
        {
            result += (actionStatistic1[action] - quotient1) * (actionStatistic2[action] - quotient2);
        }

        return result;
    }
    
    private double GetDenominatorPartForExam(
        ISet<UserAction> actionSet, 
        IDictionary<UserAction, int> actionStatistic,
        double quotient)
    {
        double result = 0;
        foreach (var action in actionSet)
        {
            result += Math.Pow(actionStatistic[action] - quotient, 2);
        }

        return Math.Sqrt(result);
    }
}
