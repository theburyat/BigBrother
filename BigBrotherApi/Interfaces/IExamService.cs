using Entities.Models;

namespace BigBrother.Interfaces;

public interface IExamService
{
    public Task<Guid> SaveExamLogAsync(
        CreateUserModel user,
        IFormFile logfile, 
        CancellationToken cancellationToken);

    public Task<UserModel> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    
    public Task<UserModel> GetUserByNameAsync(string userName, string userGroup, CancellationToken cancellationToken);

    public Task<ExamModel> GetExamAsync(Guid examId, CancellationToken cancellationToken);
    
    public Task<string> GetExamLogAsync(Guid examId, CancellationToken cancellationToken);

    public Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
    
    public Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken);
}