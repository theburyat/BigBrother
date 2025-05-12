using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface IUserProvider
{
    Task<int> CreateUserAsync(string name, int groupId, CancellationToken cancellationToken);

    Task<IEnumerable<User>> GetUsersInSessionAsync(int sessionId, CancellationToken cancellationToken);

    Task<User> GetUserAsync(int id, CancellationToken cancellationToken);
    
    Task<User> GetUserAsync(string name, int groupId, CancellationToken cancellationToken);

    Task DeleteUserAsync(int id, CancellationToken cancellationToken);

    Task<bool> IsUserExistAsync(string name, int groupId, CancellationToken cancellationToken);

    Task EnsureUserExistAsync(int id, CancellationToken cancellationToken);
}
