using BigBrother.Domain.Entities;
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
            throw new Exception();
        }
        return await _repository.CreateUserAsync(name, groupId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersBySessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);

        return await _repository.GetUsersBySessionAsync(sessionId, cancellationToken);
    }

    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserAsync(id, cancellationToken);
        
        return user ?? throw new Exception();
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        await EnsureUserExistAsync(id, cancellationToken);
        
        await _repository.DeleteUserAsync(id, cancellationToken);
    }

    public async Task EnsureUserExistAsync(int id, CancellationToken cancellationToken)
    {
        if (!await _repository.IsUserExistAsync(id, cancellationToken)) 
        {
            throw new Exception();
        }
    }
}
