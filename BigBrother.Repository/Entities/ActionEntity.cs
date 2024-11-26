using System.ComponentModel.DataAnnotations.Schema;
using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Repository.Entities;

[Table("Actions")]
public sealed class ActionEntity 
{
    public Guid Id { get; set; }

    public ActionType ActionType { get; set; }

    public DateTime DetectTime { get; set; }

    public string? Message { get; set; }

    public int SessionId { get; set; }
    public SessionEntity? Session { get; set; }

    public int UserId { get; set; }
    public UserEntity? User { get; set; }
}
