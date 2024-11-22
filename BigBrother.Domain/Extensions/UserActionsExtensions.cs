using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Extensions;

public static class UserActionsExtensions 
{
    public static IReadOnlyDictionary<ActionType, double> ToBoxCoxDistribution(this IReadOnlyDictionary<ActionType, int> userActions) 
    {
        return userActions.ToDictionary(x => x.Key, x => Math.Log(x.Value + 1));
    }
}
