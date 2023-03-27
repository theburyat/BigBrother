using Entities.Domain;

namespace BigBrother.Interfaces;

public interface ICopyAndModifyDetectionService
{
    public Task<IDictionary<Tuple<Guid, Guid>, double>> DetectCopyAndModifyAsync(List<Exam> exams, CancellationToken cancellationToken);
}
