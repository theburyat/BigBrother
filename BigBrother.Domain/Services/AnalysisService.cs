using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Services;

namespace BigBrother.Domain.Services;

public sealed class AnalysisService : IAnalysisService
{
    private readonly IDetectionService _detectionService;
    private readonly IActionProvider _actionProvider;
    private readonly IScoreProvider _scoreProvider;

    public AnalysisService(IDetectionService detectionService, IActionProvider actionProvider, IScoreProvider scoreProvider)
    {
        _detectionService = detectionService;
        _actionProvider = actionProvider;
        _scoreProvider = scoreProvider;
    }

    public async Task RunAnalysisAsync(int sessionId, CancellationToken cancellationToken)
    {
        var actions = await _actionProvider.GetSessionUsersActionsAsync(sessionId, cancellationToken);
        
        var analysisResult = await _detectionService.DetectAnomaliesAsync(actions.ToArray(), cancellationToken);
        
        var tasks = new List<Task>();
        foreach (var (userId, rating) in analysisResult) {
            var score = new Score {
                SessionId = sessionId,
                UserId = userId,
                Rating = rating
            };

            tasks.Add(Task.Run(() => _scoreProvider.AddScoreAsync(score, cancellationToken), cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
}
