using System.ComponentModel.DataAnnotations.Schema;

namespace BigBrother.Repository.Entities;

[Table("Sessions")]
public sealed class SessionEntity 
{
    public int Id { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int GroupId { get; set; }
    public GroupEntity? Group { get; set; }

    public List<ScoreEntity> Scores { get; set; } = new();
    public List<ActionEntity> Actions { get; set; } = new();
}
