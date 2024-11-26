using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface IScoreProvider 
{
    public Task<IEnumerable<Score>> GetSessionsScoresAsync(int sessionId, CancellationToken cancellationToken);

    public Task<Score> GetScoreAsync(int sessionId, int userId, CancellationToken cancellationToken);

    public Task AddScoreAsync(Score score, CancellationToken cancellationToken);
}
