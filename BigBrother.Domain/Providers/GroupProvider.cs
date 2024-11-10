using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;

namespace BigBrother.Domain.Providers;

public class GroupProvider: IGroupProvider
{
    private readonly IGroupRepository _repository;
    
    public GroupProvider(IGroupRepository repository)
    {
        _repository = repository;
    }
    
    public Task<IEnumerable<Group>> GetGroupsAsync(CancellationToken cancellationToken)
    {
        return _repository.GetGroupsAsync(cancellationToken);
    }

    public async Task<int> CreateGroupAsync(string name, CancellationToken cancellationToken)
    {
        if (await IsGroupExistAsync(name, cancellationToken))
        {
            throw new Exception();
        }

        return await _repository.CreateGroupAsync(name, cancellationToken);
    }

    public async Task DeleteGroupAsync(int id, CancellationToken cancellationToken)
    {
        if (!await IsGroupExistAsync(id, cancellationToken))
        {
            throw new Exception();
        }

        await _repository.DeleteGroupAsync(id, cancellationToken);
    }

    public Task<bool> IsGroupExistAsync(int id, CancellationToken cancellationToken)
    {
        return _repository.IsGroupExistAsync(id, cancellationToken);
    }

    public Task<bool> IsGroupExistAsync(string name, CancellationToken cancellationToken)
    {
        return _repository.IsGroupExistAsync(name, cancellationToken);
    }
}
