using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Entities;

[Table("Scores")]
[PrimaryKey(nameof(SessionId), nameof(UserId))]
public sealed class ScoreEntity 
{
    public double Rating { get; set; }

    public int SessionId { get; set; }
    public SessionEntity? Session { get; set; }

    public int UserId { get; set; }
    public UserEntity? User { get; set; }
}
