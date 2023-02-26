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
    
    public IDictionary<Tuple<Guid, Guid>, double> DetectCopyAndModify(string group, DateTime dateTime)
    {
        var usersForDetection = _userService.GetUsersFromGroup(group);

        var exams = usersForDetection.Select(user => GetUserExamAtDate(user, dateTime)).ToList();

        if (exams.Count < 2)
        {
            throw new BbException(ErrorCode.TOO_FEW_EXAMS, $"Exams count: {exams.Count}");
        }

        var detectionsResults = new Dictionary<Tuple<Guid, Guid>, double>();

        for (var i = 0; i < exams.Count - 1; i++)
        {
            for (var j = i + 1; j < exams.Count; j++)
            {
                var correlation = DetectCopyAndModifyInPair(exams[i], exams[j]);
                _logger.LogInformation($"Correlation between exams {exams[i].Id} and {exams[j].Id} is: {correlation}");
                
                detectionsResults.Add(new Tuple<Guid, Guid>(exams[i].Id, exams[j].Id), correlation);
            }
        }

        return detectionsResults;
    }

    private Exam GetUserExamAtDate(User user, DateTime dateTime)
    {
        var userExams = _examService.GetUserExams(user.Id).ToArray();
        if (userExams.Length == 0)
        {
            throw new BbException(ErrorCode.TOO_FEW_EXAMS, 
                $"User {user.Name} from group {user.Group} with id {user.Id} has no exams");
        }

        var desiredExam = userExams.LastOrDefault(x => x.Date.Date == dateTime.Date);
        if (desiredExam is null)
        {
            throw new BbException(ErrorCode.EXAM_NOT_FOUND,
                $"User {user.Name} from group {user.Group} with id {user.Id} has no exams at the date {dateTime}");
        }

        return desiredExam;
    }

    private double DetectCopyAndModifyInPair(Exam exam1, Exam exam2)
    {
        var actionStatistic1 = GetCommittedActions(exam1);
        var actionStatistic2 = GetCommittedActions(exam2);

        var correlations = new List<double>();

        for (var i = 0; i < SubsetAnalysisInPairCount; i++)
        {
            var correlation = MakeAnalysisForRandomActionsSubset(actionStatistic1, actionStatistic2);
            correlations.Add(correlation);
        }

        return correlations.Max();
    }

    private IDictionary<UserAction, int> GetCommittedActions(Exam exam)
    {
        return new Dictionary<UserAction, int>
        {
            { UserAction.Copy, exam.CopyCount },
            { UserAction.Cut, exam.CutCount },
            { UserAction.Delete, exam.DeleteCount },
            { UserAction.Enter, exam.EnterCount },
            { UserAction.Select, exam.SelectCount },
            { UserAction.Paste, exam.PasteCount },
            { UserAction.Type, exam.TypeCount },
            { UserAction.MoveCaretDown, exam.MoveCaretDownCount },
            { UserAction.MoveCaretLeft, exam.MoveCaretLeftCount },
            { UserAction.MoveCaretRight, exam.MoveCaretRightCount },
            { UserAction.MoveCaretUp, exam.MoveCaretUpCount },
            { UserAction.CompleteCode, exam.CodeCompletionCount },
            { UserAction.Run, exam.RunningCount },
            { UserAction.Build, exam.BuildingCount }
        };
    }
    
    private double MakeAnalysisForRandomActionsSubset(
        IDictionary<UserAction, int> actionStatistic1,
        IDictionary<UserAction, int> actionStatistic2)
    {
        var actionsSet = GetActionSetForAnalyse(actionStatistic1, actionStatistic2);
        if (actionsSet.Count == 0)
        {
            throw new BbException(ErrorCode.TOO_FEW_ACTIONS, $"To few actions for analysis: {actionsSet.Count}");
        }

        var quotient1 = GetQuotient(actionsSet, actionStatistic1);
        var quotient2 = GetQuotient(actionsSet, actionStatistic2);

        var numerator = GetNumerator(actionsSet, actionStatistic1, actionStatistic2, quotient1, quotient2);
        var denominator = GetDenominatorPartForExam(actionsSet, actionStatistic1, quotient1) *
                          GetDenominatorPartForExam(actionsSet, actionStatistic2, quotient2);

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
