namespace BigBrother.Domain.Entities;

public sealed class Session
{
    public int Id { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int GroupId { get; set; }

    public bool WasOpened() => StartDate != null;
    public bool WasClosed() => EndDate != null;
    public bool IsRunning() => StartDate != null && EndDate == null;
}
