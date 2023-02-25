using AutoMapper;
using Entities.Database;
using Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Services;

public class SqliteRepository: IRepository
{
    private readonly IMapper _mapper;

    public SqliteRepository(IMapper mapper)
    {
        _mapper = mapper;
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

    public bool UserWithNameExists(string userName, string userGroup)
    {
        using (var context = new AppDbContext())
        {
            return context.UserEntities.Count(x => x.Name == userName && x.Group == userGroup) > 0;
        }
    }

    public async Task<Exam> GetExamAsync(Guid examId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            return _mapper.Map<Exam>(await context.ExamEntities.FirstOrDefaultAsync(x => x.Id == examId, cancellationToken));
        }
    }

    public async Task<string?> GetExamLogAsync(Guid examId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            var exam = await context.ExamEntities.FirstOrDefaultAsync(x => x.Id == examId, cancellationToken);
            
            return exam?.ActionsLog;
        }
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

    public async Task<Guid> CreateExamAsync(Exam exam, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            await context.ExamEntities.AddAsync(_mapper.Map<ExamEntity>(exam), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return exam.Id;
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

    public async Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            var examEntity = await context.ExamEntities.FirstOrDefaultAsync(x => x.Id == examId, cancellationToken);
            context.ExamEntities.Remove(examEntity!);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return examId;
    }
}