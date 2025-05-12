namespace BigBrother.Domain.Entities;

public sealed class User
{
    public int Id { get; set; }

    public required string Name { get; set; }
    
    public int GroupId { get; set; }
}
