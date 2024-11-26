using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBrother.Repository.Entities;

[Table("Users")]
public sealed class UserEntity 
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public int GroupId { get; set; }
    public GroupEntity? Group { get; set; }

    public List<ScoreEntity> Scores { get; set; } = new();
    public List<ActionEntity> Actions { get; set; } = new();
}
