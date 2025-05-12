using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Entities;

public sealed class UserIdeActionsDistribution
{
    public required IReadOnlyDictionary<IdeActionType, int> IdeActionsDistribution { get; set; }

    public int UserId { get; set; }
}
