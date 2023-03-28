using AutoMapper;
using Entities.Database;
using Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository.Interfaces;

namespace Repository.Services;

public class UserRepository: IUserRepository
{
    private readonly IMapper _mapper;
    private readonly DbContextOptions<AppDbContext> _contextOptions;

    public UserRepository(IMapper mapper, IOptions<DbConnectionOptions> dbConnectionOptions)
    {
        _mapper = mapper;
        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(dbConnectionOptions.Value.DbConnectionString)
            .Options;
    }

    public async Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext(_contextOptions))
        {
            await context.UserEntities.AddAsync(_mapper.Map<UserEntity>(user), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return user.Id;
    }


    public async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext(_contextOptions))
        {
            return _mapper.Map<User>(await context.UserEntities.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken));
        }
    }

    public async Task<User> GetUserByNameAndGroupAsync(string userName, string userGroup, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext(_contextOptions))
        {
            return _mapper.Map<User>(await context.UserEntities.FirstOrDefaultAsync(x => x.Name == userName && x.Group == userGroup, cancellationToken));
        }
    }

    public IReadOnlyCollection<User> GetUsersFromGroup(string userGroup)
    {
        using (var context = new AppDbContext(_contextOptions))
        {
            return _mapper.Map<IReadOnlyCollection<User>>(context.UserEntities.Where(x => x.Group == userGroup));
        }
    }

    public bool UserWithNameFromGroupExists(string userName, string userGroup)
    {
        using (var context = new AppDbContext(_contextOptions))
        {
            return context.UserEntities.Count(x => x.Name == userName && x.Group == userGroup) > 0;
        }
    }
    
    public async Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext(_contextOptions))
        {
            var userEntity = await context.UserEntities.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            context.UserEntities.Remove(userEntity!);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return userId;
    }
}
