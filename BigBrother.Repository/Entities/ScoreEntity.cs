using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Entities;

[Table("Scores")]
[PrimaryKey(nameof(SessionId), nameof(UserId))]
internal sealed class ScoreEntity 
{
    internal double Rating { get; set; }

    internal int SessionId { get; set; }
    internal SessionEntity? Session { get; set; }

    internal int UserId { get; set; }
    internal UserEntity? User { get; set; }
}
