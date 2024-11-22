using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBrother.Repository.Entities;

[Table("Groups")]
internal sealed class GroupEntity 
{
    internal int Id { get; set; }

    [Required]
    internal required string Name { get; set; }

    internal List<UserEntity> Users { get; set; } = new();
    internal List<SessionEntity> Sessions { get; set; } = new();
}
