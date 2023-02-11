namespace Entities.Domain;

public class User
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string Group { get; set; }
}