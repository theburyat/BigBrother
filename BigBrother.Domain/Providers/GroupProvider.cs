using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Entities.Exceptions;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;

namespace BigBrother.Domain.Providers;

public sealed class GroupProvider: IGroupProvider
{
    private readonly IGroupRepository _repository;
    
    public GroupProvider(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> CreateGroupAsync(string name, CancellationToken cancellationToken)
    {
        if (await _repository.IsGroupExistAsync(name, cancellationToken))
        {
            throw new BadRequestException(ErrorCode.GroupAlreadyExists, $"Group with name '{name}' already exists");
        }

        return await _repository.CreateGroupAsync(name, cancellationToken);
    }
    
    public Task<IEnumerable<Group>> GetGroupsAsync(CancellationToken cancellationToken)
    {
        return _repository.GetGroupsAsync(cancellationToken);
    }

    public async Task<Group> GetGroupAsync(int id, CancellationToken cancellationToken)
    {
        return await _repository.GetGroupAsync(id, cancellationToken)
            ?? throw new BadRequestException(ErrorCode.GroupNotFound, $"Group with id '{id}' was not found");
    }

    public async Task DeleteGroupAsync(int id, CancellationToken cancellationToken)
    {
        await EnsureGroupExistAsync(id, cancellationToken);

        await _repository.DeleteGroupAsync(id, cancellationToken);
    }

    public async Task EnsureGroupExistAsync(int id, CancellationToken cancellationToken)
    {
        if (!await _repository.IsGroupExistAsync(id, cancellationToken)) 
        {
            throw new BadRequestException(ErrorCode.GroupNotFound, $"Group with id '{id}' was not found");
        }
    }
}
