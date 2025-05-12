using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Repositories;
using BigBrother.Repository.Context.Factory;
using BigBrother.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IContextFactory _contextFactory;

    public UserRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> CreateUserAsync(string name, int groupId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var userEntity = new UserEntity 
        {
            Name = name,
            GroupId = groupId
        };
        await context.Users.AddAsync(userEntity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return userEntity.Id;
    }

    public async Task<IEnumerable<User>> GetUsersInSessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var groupId = await context.Sessions
            .AsNoTracking()
            .Where(x => x.Id == sessionId)
            .Select(x => x.GroupId)
            .FirstAsync(cancellationToken);

        return await context.Users
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .Select(x => new User 
            {
                Id = x.Id,
                Name = x.Name,
                GroupId = x.GroupId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null
            ? null
            : new User
            {
                Id = entity.Id,
                Name = entity.Name,
                GroupId = entity.GroupId
            };
    }

    public async Task<User?> GetUserAsync(string name, int groupId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && x.GroupId == groupId, cancellationToken);

        return entity == null
            ? null
            : new User
            {
                Id = entity.Id,
                Name = entity.Name,
                GroupId = entity.GroupId
            };
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();
         
        await context.Users
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();
        
        return await context.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> IsUserExistAsync(string name, int groupId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();
        
        return await context.Users
            .AsNoTracking()
            .AnyAsync(x => name == x.Name && x.GroupId == groupId, cancellationToken);
    }
}
