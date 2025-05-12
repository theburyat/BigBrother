using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface IGroupRepository
{
    Task<int> CreateGroupAsync(string name, CancellationToken cancellationToken);

    Task<IEnumerable<Group>> GetGroupsAsync(CancellationToken cancellationToken);

    Task<Group?> GetGroupAsync(int id, CancellationToken cancellationToken);

    Task DeleteGroupAsync(int id, CancellationToken cancellationToken);

    Task<bool> IsGroupExistAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> IsGroupExistAsync(string name, CancellationToken cancellationToken);
}
