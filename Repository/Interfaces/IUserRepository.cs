using Entities.Domain;

namespace Repository.Interfaces;

public interface IUserRepository
{
    public Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken);
    
    public Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken);

    public Task<User> GetUserByNameAsync(string userName, string userGroup, CancellationToken cancellationToken);

    public IReadOnlyCollection<User> GetUsersFromGroup(string userGroup);

    public bool UserWithNameExists(string userName, string userGroup);
    
    public Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
}
