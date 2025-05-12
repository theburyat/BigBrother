namespace BigBrother.Domain.Interfaces.Services;

public interface IAnalysisService 
{
    Task RunAnalysisAsync(int sessionId, CancellationToken cancellationToken);
}
