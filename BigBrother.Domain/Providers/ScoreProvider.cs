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

    public async Task<IEnumerable<Score>> GetSessionsScoresAsync(int sessionId, CancellationToken cancellationToken)
    {
        if (!await _sessionProvider.IsSessionExistAsync(sessionId, cancellationToken)) {
            throw new Exception();
        }
        return await _repository.GetSessionsScoresAsync(sessionId, cancellationToken);
    }

    public async Task AddScoreAsync(Score score, CancellationToken cancellationToken)
    {
        await ValidateScoreAsync(score, cancellationToken);
        await _repository.AddScoreAsync(score, cancellationToken);
    }

    private async Task ValidateScoreAsync(Score score, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(score);
        
        if (!await _sessionProvider.IsSessionExistAsync(score.SessionId, cancellationToken)) {
            throw new Exception();
        }
        if (!await _userProvider.IsUserExistAsync(score.UserId, cancellationToken)) {
            throw new Exception();
        }
        // TODO check if session group id = user group id
    }
}
