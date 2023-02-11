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
    
    public async Task<User> GetUserAsync(Guid userId, AppDbContext context, CancellationToken cancellationToken)
    {
        return _mapper.Map<User>(await context.UserEntities.SingleAsync(x => x.Id == userId, cancellationToken));
    }

    public async Task<Exam> GetExamAsync(Guid examId, AppDbContext context, CancellationToken cancellationToken)
    {
        return _mapper.Map<Exam>(await context.ExamEntities.SingleAsync(x => x.Id == examId, cancellationToken));
    }

    public async Task<Guid> CreateUserAsync(User user, AppDbContext context, CancellationToken cancellationToken)
    {
        await context.UserEntities.AddAsync(_mapper.Map<UserEntity>(user), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return user.Id;
    }

    public async Task<Guid> CreateExamAsync(Exam exam, AppDbContext context, CancellationToken cancellationToken)
    {
        await context.ExamEntities.AddAsync(_mapper.Map<ExamEntity>(exam), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return exam.Id;
    }

    public async Task<Guid> DeleteUserAsync(Guid userId, AppDbContext context, CancellationToken cancellationToken)
    {
        var userEntity = await context.UserEntities.SingleAsync(x => x.Id == userId, cancellationToken); 
        context.UserEntities.Remove(userEntity);
        await context.SaveChangesAsync(cancellationToken);

        return userId;
    }

    public async Task<Guid> DeleteExamAsync(Guid examId, AppDbContext context, CancellationToken cancellationToken)
    {
        var examEntity = await context.ExamEntities.SingleAsync(x => x.Id == examId, cancellationToken);
        context.ExamEntities.Remove(examEntity);
        await context.SaveChangesAsync(cancellationToken);

        return examId;
    }
}