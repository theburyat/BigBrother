namespace BigBrother.Domain.Entities;

public sealed class Score 
{
    public double Rating { get; set; }

    public int SessionId { get; set;}
    
    public int UserId { get; set; }
}
