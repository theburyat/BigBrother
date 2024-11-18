using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;

namespace BigBrother.Domain.Providers;

public sealed class UserProvider : IUserProvider
{
    private readonly IUserRepository _repository;
    private readonly IGroupProvider _groupProvider;

    public UserProvider(IUserRepository repository, IGroupProvider groupProvider)
    {
        _repository = repository;
        _groupProvider = groupProvider;
    }

    public async Task<bool> IsUserExistAsync(string name, string group, CancellationToken cancellationToken)
    {
        if (!await _groupProvider.IsGroupExistAsync(group, cancellationToken)) {
            throw new Exception();
        }
        return await _repository.IsUserExistAsync(name, group, cancellationToken);
    }

    public async Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken)
    {
        return await _repository.IsUserExistAsync(id, cancellationToken);
    }

    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        await ValidateUserExistingAsync(id, cancellationToken);
        return await _repository.GetUserAsync(id, cancellationToken);
    }

    public async Task<User> GetUserAsync(string name, string group, CancellationToken cancellationToken)
    {
        if (!await IsUserExistAsync(name, group, cancellationToken)) {
            throw new Exception();
        }
        return await _repository.GetUserAsync(name, group, cancellationToken);
    }

    public async Task CreateUserAsync(string name, string group, CancellationToken cancellationToken)
    {
        if (await IsUserExistAsync(name, group, cancellationToken)) {
            throw new Exception();
        }
        await _repository.CreateUserAsync(name, group, cancellationToken);
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        await ValidateUserExistingAsync(id, cancellationToken);
        await _repository.DeleteUserAsync(id, cancellationToken);
    }

    private async Task ValidateUserExistingAsync(int id, CancellationToken cancellationToken) {
        if (!await IsUserExistAsync(id, cancellationToken)) {
            throw new Exception();
        }
    }
}
