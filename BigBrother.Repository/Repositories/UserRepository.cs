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
        
        if (entity == null) 
        {
            return null;
        }

        return new User 
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
            .ExecuteDeleteAsync();
    }

    public async Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();
        
        return await context.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == id);
    }

    public async Task<bool> IsUserExistAsync(string name, int groupId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();
        
        return await context.Users
            .AsNoTracking()
            .AnyAsync(x => string.Equals(x.Name, name, StringComparison.Ordinal) && x.GroupId == groupId, cancellationToken);
    }
}
