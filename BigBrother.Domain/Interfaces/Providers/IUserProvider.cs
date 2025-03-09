using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface IUserProvider
{
    public Task<int> CreateUserAsync(string name, int groupId, CancellationToken cancellationToken);

    public Task<IEnumerable<User>> GetUsersInSessionAsync(int sessionId, CancellationToken cancellationToken);

    public Task<User> GetUserAsync(int id, CancellationToken cancellationToken);

    public Task DeleteUserAsync(int id, CancellationToken cancellationToken);

    public Task EnsureUserExistAsync(int id, CancellationToken cancellationToken);
}
