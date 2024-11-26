using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface ISessionProvider
{
    Task<IEnumerable<Session>> GetGroupSessionsAsync(int groupId, CancellationToken cancellationToken);
    
    Task<Session> GetSessionAsync(int id, CancellationToken cancellationToken);

    Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken);

    Task DeleteSessionAsync(int id, CancellationToken cancellationToken);

    Task StartSessionAsync(int id, CancellationToken cancellationToken);
    
    Task StopSessionAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> IsSessionExistAsync(int id, CancellationToken cancellationToken);
}
