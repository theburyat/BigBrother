using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Services;

namespace BigBrother.Domain.Services;

public sealed class AnalysisService : IAnalysisService
{
    private readonly IDetectionService _detectionService;
    private readonly IActionProvider _actionProvider;

    public AnalysisService(IDetectionService detectionService, IActionProvider actionProvider)
    {
        _detectionService = detectionService;
        _actionProvider = actionProvider;
    }

    public async Task<IDictionary<int, double>> RunAnalysisAsync(int sessionId, CancellationToken cancellationToken)
    {
        var actions = await _actionProvider.GetSessionUsersActionsAsync(sessionId, cancellationToken);
        
        return await _detectionService.DetectAnomaliesAsync(actions.ToArray(), cancellationToken);
    }
}
