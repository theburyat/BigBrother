using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Entities;

public class IdeAction {
    public Guid Id { get; set; }

    public ActionType ActionType { get; set; }
    
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public string Message { get; set; }
}
