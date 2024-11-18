namespace BigBrother.Domain.Entities;

public sealed class Score 
{
    public int SessionId { get; set;}
    
    public int UserId { get; set; }

    public double Rating { get; set; }
}
