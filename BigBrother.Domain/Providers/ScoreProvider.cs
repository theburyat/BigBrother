using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Entities.Exceptions;
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
            throw new BadRequestException(ErrorCode.InvalidScore, "Score rating cannot be negative");
        }

        await _sessionProvider.EnsureSessionExistAsync(score.SessionId, cancellationToken);
        await _userProvider.EnsureUserExistAsync(score.UserId, cancellationToken);
        
        await _repository.AddScoreAsync(score, cancellationToken);
    }

    public async Task<IEnumerable<Score>> GetScoresInSessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);
        
        return await _repository.GetScoresInSessionAsync(sessionId, cancellationToken);
    }

    public async Task<Score> GetScoreAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);
        await _userProvider.EnsureUserExistAsync(userId, cancellationToken);

        return await _repository.GetScoreAsync(sessionId, userId, cancellationToken)
            ?? throw new BadRequestException(ErrorCode.ScoreNotFound, $"Score of user id '{userId}' in session '{sessionId}' was not found");
    }
}
