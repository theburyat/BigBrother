namespace BigBrother.Domain.Interfaces.Services;

public interface IAnalysisService {
    Task<IDictionary<int, double>> RunAnalysisAsync(int sessionId, CancellationToken cancellationToken);
}
