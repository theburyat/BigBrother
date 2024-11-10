using BigBrother.Domain.Entities;

namespace BigBrother.Domain.RepositoryInterfaces;

public interface IScoreRepository {
    public Task<IEnumerable<Score>> GetSessionsScoresAsync(int sessionId, CancellationToken cancellationToken);

    public Task AddScoreAsync(Score score, CancellationToken cancellationToken);
}
