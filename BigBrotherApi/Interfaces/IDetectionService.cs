namespace BigBrother.Interfaces;

public interface IDetectionService
{
    public Task<string> RunAnalysisAsync(string dateLine, string group, CancellationToken cancellationToken);
}