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

    public async Task<User?> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        var entity = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null) {
            return null;
        }

        return new User {
            Id = entity.Id,
            Name = entity.Name,
            GroupId = entity.GroupId
        };
    }

    public async Task<User?> GetUserAsync(string name, string group, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        var entity = await context.Users
            .AsNoTracking()
            .Include(x => x.Group)
            .FirstOrDefaultAsync(x => string.Equals(x.Name, name, StringComparison.Ordinal)
                                      && string.Equals(x.Group!.Name, group, StringComparison.Ordinal), 
                                 cancellationToken);

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

    public async Task CreateUserAsync(string name, string group, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        var groupEntity = await context.Groups.FirstAsync(x => string.Equals(x.Name, group, StringComparison.Ordinal), cancellationToken);
        var userEntity = new UserEntity {
            Name = name,
            GroupId = groupEntity.Id
        };

        await context.Users.AddAsync(userEntity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();
         
        return context.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();
        
        return context.Users.AnyAsync(x => x.Id == id);
    }

    public Task<bool> IsUserExistAsync(string name, string group, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();
        
        return context.Users.Include(x => x.Group).AnyAsync(x => string.Equals(x.Name, name, StringComparison.Ordinal)
                                                                 && string.Equals(x.Group!.Name, group, StringComparison.Ordinal));
    }
}
