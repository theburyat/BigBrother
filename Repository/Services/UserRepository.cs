using AutoMapper;
using Entities.Database;
using Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Services;

public class UserRepository: IUserRepository
{
    private readonly IMapper _mapper;

    public UserRepository(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            await context.UserEntities.AddAsync(_mapper.Map<UserEntity>(user), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return user.Id;
    }


    public async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            return _mapper.Map<User>(await context.UserEntities.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken));
        }
    }

    public async Task<User> GetUserByNameAsync(string userName, string userGroup, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            return _mapper.Map<User>(await context.UserEntities.FirstOrDefaultAsync(x => x.Name == userName && x.Group == userGroup, cancellationToken));
        }
    }

    public IReadOnlyCollection<User> GetUsersFromGroup(string userGroup)
    {
        using (var context = new AppDbContext())
        {
            return _mapper.Map<IReadOnlyCollection<User>>(context.UserEntities.Where(x => x.Group == userGroup));
        }
    }

    public bool UserWithNameExists(string userName, string userGroup)
    {
        using (var context = new AppDbContext())
        {
            return context.UserEntities.Count(x => x.Name == userName && x.Group == userGroup) > 0;
        }
    }
    
    public async Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            var userEntity = await context.UserEntities.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            context.UserEntities.Remove(userEntity!);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return userId;
    }
}
