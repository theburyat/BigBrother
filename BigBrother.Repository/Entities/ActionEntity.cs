using System.ComponentModel.DataAnnotations.Schema;
using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Repository.Entities;

[Table("Actions")]
internal sealed class ActionEntity 
{
    internal Guid Id { get; set; }

    internal ActionType ActionType { get; set; }

    internal string? Message { get; set; }

    internal int SessionId { get; set; }
    internal SessionEntity? Session { get; set; }

    internal int UserId { get; set; }
    internal UserEntity? User { get; set; }
}
