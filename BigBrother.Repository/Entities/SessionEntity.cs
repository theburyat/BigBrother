using System.ComponentModel.DataAnnotations.Schema;

namespace BigBrother.Repository.Entities;

[Table("Sessions")]
internal sealed class SessionEntity 
{
    internal int Id { get; set; }

    internal DateTime? StartDate { get; set; }

    internal DateTime? EndDate { get; set; }

    internal int GroupId { get; set; }
    internal GroupEntity? Group { get; set; }

    internal List<ScoreEntity> Scores { get; set; } = new();
    internal List<ActionEntity> Actions { get; set; } = new();
}
