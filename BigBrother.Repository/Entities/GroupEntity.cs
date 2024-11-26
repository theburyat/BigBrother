using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBrother.Repository.Entities;

[Table("Groups")]
public sealed class GroupEntity 
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public List<UserEntity> Users { get; set; } = new();
    public List<SessionEntity> Sessions { get; set; } = new();
}
