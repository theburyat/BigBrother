using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<int> CreateUserAsync(string name, int groupId, CancellationToken cancellationToken);

    public Task<IEnumerable<User>> GetUsersInSessionAsync(int sessionId, CancellationToken cancellationToken);

    public Task<User?> GetUserAsync(int id, CancellationToken cancellationToken);
    
    public Task<User?> GetUserAsync(string name, int groupId, CancellationToken cancellationToken);

    public Task DeleteUserAsync(int id, CancellationToken cancellationToken);

    public Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken);

    public Task<bool> IsUserExistAsync(string name, int groupId, CancellationToken cancellationToken);
}
