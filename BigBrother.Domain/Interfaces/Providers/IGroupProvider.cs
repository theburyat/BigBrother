using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface IGroupProvider
{
    Task<IEnumerable<Group>> GetGroupsAsync(CancellationToken cancellationToken);

    Task<int> CreateGroupAsync(string name, CancellationToken cancellationToken);

    Task DeleteGroupAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> IsGroupExistAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> IsGroupExistAsync(string name, CancellationToken cancellationToken);
}
