using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Extensions;

public static class UserActionsExtensions {
    public static IReadOnlyDictionary<IdeActionType, double> ToBoxCoxDistribution(this IReadOnlyDictionary<IdeActionType, int> userActions) {
        return userActions.ToDictionary(x => x.Key, x => Math.Log(x.Value + 1));
    }
}
