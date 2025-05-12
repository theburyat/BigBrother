using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface IGroupProvider
{
    Task<int> CreateGroupAsync(string name, CancellationToken cancellationToken);

    Task<IEnumerable<Group>> GetGroupsAsync(CancellationToken cancellationToken);
    
    Task<Group> GetGroupAsync(int id, CancellationToken cancellationToken);

    Task DeleteGroupAsync(int id, CancellationToken cancellationToken);
    
    Task EnsureGroupExistAsync(int id, CancellationToken cancellationToken);
}
