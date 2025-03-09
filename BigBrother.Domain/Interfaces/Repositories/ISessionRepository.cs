using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface ISessionRepository
{
    Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken);

    Task<IEnumerable<Session>> GetSessionsInGroupAsync(int groupId, CancellationToken cancellationToken);

    Task<Session?> GetSessionAsync(int id, CancellationToken cancellationToken);

    Task DeleteSessionAsync(int id, CancellationToken cancellationToken);

    Task StartSessionAsync(int id, CancellationToken cancellationToken);
    
    Task StopSessionAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> IsSessionExistAsync(int id, CancellationToken cancellationToken);
}
