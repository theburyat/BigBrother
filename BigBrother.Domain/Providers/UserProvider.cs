using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Entities.Exceptions;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;

namespace BigBrother.Domain.Providers;

public sealed class UserProvider : IUserProvider
{
    private readonly IUserRepository _repository;
    private readonly IGroupProvider _groupProvider;
    private readonly ISessionProvider _sessionProvider;

    public UserProvider(IUserRepository repository, IGroupProvider groupProvider, ISessionProvider sessionProvider)
    {
        _repository = repository;
        _groupProvider = groupProvider;
        _sessionProvider = sessionProvider;
    }

    public async Task<int> CreateUserAsync(string name, int groupId, CancellationToken cancellationToken)
    {
        await _groupProvider.EnsureGroupExistAsync(groupId, cancellationToken);

        if (await _repository.IsUserExistAsync(name, groupId, cancellationToken)) 
        {
            throw new BadRequestException(ErrorCode.UserAlreadyExists, $"User with name '{name}' already exists in group with id '{groupId}'");
        }
        return await _repository.CreateUserAsync(name, groupId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersInSessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);

        return await _repository.GetUsersInSessionAsync(sessionId, cancellationToken);
    }

    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        return await _repository.GetUserAsync(id, cancellationToken) 
            ?? throw new BadRequestException(ErrorCode.UserNotFound, $"User with id '{id}' was not found");
    }

    public async Task<User> GetUserAsync(string name, int groupId, CancellationToken cancellationToken)
    {
        await _groupProvider.EnsureGroupExistAsync(groupId, cancellationToken);
        
        return await _repository.GetUserAsync(name, groupId, cancellationToken) 
            ?? throw new BadRequestException(ErrorCode.UserNotFound, $"User with name '{name}' was not found in group with id '{groupId}'");
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        await EnsureUserExistAsync(id, cancellationToken);
        
        await _repository.DeleteUserAsync(id, cancellationToken);
    }

    public Task<bool> IsUserExistAsync(string name, int groupId, CancellationToken cancellationToken)
    {
        _groupProvider.EnsureGroupExistAsync(groupId, cancellationToken);
        
        return _repository.IsUserExistAsync(name, groupId, cancellationToken);
    }

    public async Task EnsureUserExistAsync(int id, CancellationToken cancellationToken)
    {
        if (!await _repository.IsUserExistAsync(id, cancellationToken)) 
        {
            throw new BadRequestException(ErrorCode.UserNotFound, $"User with id '{id}' was not found");
        }
    }
}
