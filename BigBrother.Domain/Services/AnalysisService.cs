using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Services;

namespace BigBrother.Domain.Services;

public sealed class AnalysisService : IAnalysisService
{
    private readonly IDetectionService _detectionService;
    private readonly ISessionProvider _sessionProvider;
    private readonly IActionProvider _actionProvider;
    private readonly IScoreProvider _scoreProvider;

    public AnalysisService(IDetectionService detectionService, ISessionProvider sessionProvider, IActionProvider actionProvider, IScoreProvider scoreProvider)
    {
        _detectionService = detectionService;
        _sessionProvider = sessionProvider;
        _actionProvider = actionProvider;
        _scoreProvider = scoreProvider;
    }

    public async Task RunAnalysisAsync(int sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessionProvider.GetSessionAsync(sessionId, cancellationToken);
        if (session.StartDate == null) 
        {
            throw new Exception();
        }
        if (session.EndDate == null) 
        {
            throw new Exception();
        }

        var actions = await _actionProvider.GetUserIdeActionDistributionsInSessionAsync(sessionId, cancellationToken);
        if (actions.Count() == 0) 
        {
            throw new Exception();
        }
        
        var analysisResult = await _detectionService.DetectAnomaliesAsync(actions.ToArray(), cancellationToken);
        
        var tasks = new List<Task>();
        foreach (var (userId, rating) in analysisResult) 
        {
            var score = new Score 
            {
                Rating = rating,
                SessionId = sessionId,
                UserId = userId
            };

            tasks.Add(Task.Run(() => _scoreProvider.AddScoreAsync(score, cancellationToken), cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
}
