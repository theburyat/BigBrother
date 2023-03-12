namespace BigBrother.Interfaces;

public interface IGenerationFromScratchDetectionService
{
    public Task<IDictionary<Guid, double>> DetectGenerationFromScratchAsync(string group, DateTime dateTime, CancellationToken cancellationToken);
}