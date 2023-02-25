using Entities.Domain;

namespace Repository.Interfaces;

public interface IRepository
{
    public Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken);

    public Task<User> GetUserByNameAsync(string userName, string userGroup, CancellationToken cancellationToken);

    public bool UserWithNameExists(string userName, string userGroup);

    public Task<Exam> GetExamAsync(Guid examId, CancellationToken cancellationToken);

    public Task<string?> GetExamLogAsync(Guid examId, CancellationToken cancellationToken);

    public Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken);

    public Task<Guid> CreateExamAsync(Exam exam, CancellationToken cancellationToken);

    public Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);

    public Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken);
}