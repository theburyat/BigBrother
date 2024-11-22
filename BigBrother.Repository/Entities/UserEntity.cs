using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBrother.Repository.Entities;

[Table("Users")]
internal sealed class UserEntity 
{
    internal int Id { get; set; }

    [Required]
    internal required string Name { get; set; }

    internal int GroupId { get; set; }
    internal GroupEntity? Group { get; set; }

    internal List<ScoreEntity> Scores { get; set; } = new();
    internal List<ActionEntity> Actions { get; set; } = new();
}
