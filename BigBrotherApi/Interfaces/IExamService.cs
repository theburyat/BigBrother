using Entities.Domain;
using Entities.Models;

namespace BigBrother.Interfaces;

public interface IExamService
{
    public Task<Guid> SaveExamLogAsync(
        CreateUserModel user,
        IFormFile logfile, 
        CancellationToken cancellationToken);

    public Task<Exam> GetExamAsync(Guid examId, CancellationToken cancellationToken);
    
    public Task<string> GetExamLogAsync(Guid examId, CancellationToken cancellationToken);
    
    public IEnumerable<Exam> GetUserExams(Guid userId);

    public Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken);
}