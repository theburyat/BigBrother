using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;

namespace BigBrother.Domain.Providers;

public sealed class ScoreProvider : IScoreProvider
{
    private readonly IScoreRepository _repository;
    private readonly ISessionProvider _sessionProvider;
    private readonly IUserProvider _userProvider;

    public ScoreProvider(IScoreRepository repository, ISessionProvider sessionProvider, IUserProvider userProvider)
    {
        _repository = repository;
        _sessionProvider = sessionProvider;
        _userProvider = userProvider;
    }

    public async Task AddScoreAsync(Score score, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(score);
        
        if (score.Rating < 0) 
        {
            throw new Exception();
        }

        await _sessionProvider.EnsureSessionExistAsync(score.SessionId, cancellationToken);
        await _userProvider.EnsureUserExistAsync(score.UserId, cancellationToken);
        // to do check if user and session have equal group id
        // to do check if already have score with same user id in session
        
        await _repository.AddScoreAsync(score, cancellationToken);
    }

    public async Task<IEnumerable<Score>> GetScoresBySessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);
        
        return await _repository.GetScoresBySessionAsync(sessionId, cancellationToken);
    }

    public async Task<Score> GetScoreAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);
        await _userProvider.EnsureUserExistAsync(userId, cancellationToken);
        // to do check if user and session have equal group id

        var score = await _repository.GetScoreAsync(sessionId, userId, cancellationToken);
        return score ?? throw new Exception();
    }
}
