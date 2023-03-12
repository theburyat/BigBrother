using Entities.Domain;
using Entities.Models;

namespace BigBrother.Interfaces;

public interface IUserService
{
    public Task<Guid> GetUserIdByNameWithCreationIfNotExistAsync(CreateUserModel userModel, CancellationToken cancellationToken);
    
    public Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    
    public Task<User> GetUserByNameAsync(string userName, string userGroup, CancellationToken cancellationToken);

    public IReadOnlyCollection<User> GetUsersFromGroup(string userGroup);

    public Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
}
