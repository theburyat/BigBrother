using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface ISessionRepository
{
    Task<IEnumerable<Session>> GetGroupSessionsAsync(int groupId, CancellationToken cancellationToken);

    Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken);

    Task<int> DeleteSessionAsync(int id, CancellationToken cancellationToken);

    Task StartSessionAsync(int id, CancellationToken cancellationToken);
    
    Task StopSessionAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> IsSessionExistAsync(int id, CancellationToken cancellationToken);
}
