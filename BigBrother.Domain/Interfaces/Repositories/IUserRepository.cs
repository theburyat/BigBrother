using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetSessionUsersAsync(int sessionId, CancellationToken cancellationToken);

    public Task<User?> GetUserAsync(int id, CancellationToken cancellationToken);
    
    public Task<User?> GetUserAsync(string name, string group, CancellationToken cancellationToken);

    public Task CreateUserAsync(string name, string group, CancellationToken cancellationToken);

    public Task DeleteUserAsync(int id, CancellationToken cancellationToken);

    public Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken);

    public Task<bool> IsUserExistAsync(string name, string group, CancellationToken cancellationToken);
}
