using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Entities;

public sealed class UserActions
{
    public int UserId { get; set; }

    public required IReadOnlyDictionary<ActionType, int> Actions { get; set; }
}
