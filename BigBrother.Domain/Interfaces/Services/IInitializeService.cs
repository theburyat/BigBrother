namespace BigBrother.Domain.Interfaces.Services;

public interface IInitializeService
{
    Task InitializeAsync(CancellationToken cancellationToken);
}