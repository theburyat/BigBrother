using System.ComponentModel.DataAnnotations.Schema;
using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Repository.Entities;

[Table("IdeActions")]
public sealed class IdeActionEntity 
{
    public Guid Id { get; set; }

    public IdeActionType ActionType { get; set; }

    public string? Message { get; set; }

    public DateTime DetectTime { get; set; }

    public int SessionId { get; set; }
    public SessionEntity? Session { get; set; }

    public int UserId { get; set; }
    public UserEntity? User { get; set; }
}
