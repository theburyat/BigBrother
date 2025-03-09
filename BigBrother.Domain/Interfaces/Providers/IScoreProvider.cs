using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface IScoreProvider 
{
    public Task AddScoreAsync(Score score, CancellationToken cancellationToken);

    public Task<IEnumerable<Score>> GetScoresInSessionAsync(int sessionId, CancellationToken cancellationToken);

    public Task<Score> GetScoreAsync(int sessionId, int userId, CancellationToken cancellationToken);
}
