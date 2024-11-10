using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface IScoreRepository {
    public Task<IEnumerable<Score>> GetSessionsScoresAsync(int sessionId, CancellationToken cancellationToken);

    public Task AddScoreAsync(Score score, CancellationToken cancellationToken);
}
