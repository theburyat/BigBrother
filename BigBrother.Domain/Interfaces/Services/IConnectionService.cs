namespace BigBrother.Domain.Interfaces.Services;

public interface IConnectionService
{
    Task<int> ConnectAsync(string username, int sessionId, CancellationToken cancellationToken);
}