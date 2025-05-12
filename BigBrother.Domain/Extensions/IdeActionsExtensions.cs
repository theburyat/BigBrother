using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Extensions;

public static class IdeActionsDistributionExtensions 
{
    public static IReadOnlyDictionary<IdeActionType, double> ToBoxCoxDistribution(this IReadOnlyDictionary<IdeActionType, int> distribution) 
    {
        return distribution.ToDictionary(x => x.Key, x => Math.Log(x.Value + 1));
    }
}
