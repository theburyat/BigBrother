using Entities.Domain;
using Entities.Enums;

namespace BigBrother.Extensions;

public static class ExamExtensions
{
    public static IDictionary<UserAction, int> GetCommittedActions(this Exam exam, bool addTypeAction = true)
    {
        var result =  new Dictionary<UserAction, int>
        {
            { UserAction.Copy, exam.CopyCount },
            { UserAction.Cut, exam.CutCount },
            { UserAction.Delete, exam.DeleteCount },
            { UserAction.Enter, exam.EnterCount },
            { UserAction.Select, exam.SelectCount },
            { UserAction.Paste, exam.PasteCount },
            { UserAction.MoveCaretDown, exam.MoveCaretDownCount },
            { UserAction.MoveCaretLeft, exam.MoveCaretLeftCount },
            { UserAction.MoveCaretRight, exam.MoveCaretRightCount },
            { UserAction.MoveCaretUp, exam.MoveCaretUpCount },
            { UserAction.CompleteCode, exam.CodeCompletionCount },
            { UserAction.Run, exam.RunningCount },
            { UserAction.Build, exam.BuildingCount }
        };

        if (addTypeAction)
        {
            result.Add(UserAction.Type, exam.TypeCount);
        }

        return result;
    }

    public static IDictionary<UserAction, double> TransformToBoxCoxDistribution(this IDictionary<UserAction, int> committedAction)
    {
        return committedAction
            .Select(x => new KeyValuePair<UserAction, double>(x.Key, Math.Log(x.Value + 1))) // log base e
            .ToDictionary(x => x.Key, x => x.Value);
    }
}