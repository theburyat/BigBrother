using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Entities;

public sealed class IdeAction 
{
    public Guid Id { get; set; }

    public IdeActionType Type { get; set; }

    public string? Message { get; set; }

    public DateTime DetectTime { get; set; }
    
    public int SessionId { get; set; }

    public int UserId { get; set; }
}
