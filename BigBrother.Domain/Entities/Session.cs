namespace BigBrother.Domain.Entities;

public sealed class Session
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
