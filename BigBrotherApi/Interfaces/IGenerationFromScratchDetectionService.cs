using Entities.Domain;

namespace BigBrother.Interfaces;

public interface IGenerationFromScratchDetectionService
{
    public Task<IDictionary<Guid, double>> DetectGenerationFromScratchAsync(List<Exam> exams, CancellationToken cancellationToken);
}