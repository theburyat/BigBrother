using Entities.Domain;

namespace Repository.Interfaces;

public interface IRepository
{
    public Task<User> GetUserAsync(Guid userId, AppDbContext context, CancellationToken cancellationToken);

    public Task<Exam> GetExamAsync(Guid examId, AppDbContext context, CancellationToken cancellationToken);

    public Task<Guid> CreateUserAsync(User user, AppDbContext context, CancellationToken cancellationToken);

    public Task<Guid> CreateExamAsync(Exam exam, AppDbContext context, CancellationToken cancellationToken);

    public Task<Guid> DeleteUserAsync(Guid userId, AppDbContext context, CancellationToken cancellationToken);

    public Task<Guid> DeleteExamAsync(Guid examId, AppDbContext context, CancellationToken cancellationToken);
}