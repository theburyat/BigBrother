using System.Data;
using BigBrother.Domain.Entities;

namespace BigBrother.Domain.ProviderInterfaces;

public interface ISessionProvider
{
    Task<bool> IsSessionExistAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Session>> GetGroupSessionsAsync(int groupId, CancellationToken cancellationToken);

    Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken);

    Task DeleteSessionAsync(int id, CancellationToken cancellationToken);

    Task StartSessionAsync(int id, CancellationToken cancellationToken);
    
    Task StopSessionAsync(int id, CancellationToken cancellationToken);
}
