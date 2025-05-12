using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface ISessionProvider
{
    Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken);

    Task<IEnumerable<Session>> GetSessionsInGroupAsync(int groupId, CancellationToken cancellationToken);
    
    Task<Session> GetSessionAsync(int id, CancellationToken cancellationToken);

    Task DeleteSessionAsync(int id, CancellationToken cancellationToken);

    // TODO remake to status
    Task StartSessionAsync(int id, CancellationToken cancellationToken);
    
    Task StopSessionAsync(int id, CancellationToken cancellationToken);
    
    Task EnsureSessionExistAsync(int id, CancellationToken cancellationToken);
}
