using Entities.Domain;

namespace Repository.Interfaces;

public interface IExamRepository
{
    public Task<Guid> CreateExamAsync(Exam exam, CancellationToken cancellationToken);
    
    public Task<Exam> GetExamAsync(Guid examId, CancellationToken cancellationToken);

    public IEnumerable<Exam> GetUserExams(Guid userId);

    public Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken);
}
