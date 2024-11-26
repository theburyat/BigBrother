global using Action = BigBrother.Domain.Entities.Action;

using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Entities;

public sealed class Action 
{
    public Guid Id { get; set; }

    public ActionType ActionType { get; set; }

    public DateTime DetectTime { get; set; }
    
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public string? Message { get; set; }
}
