using BigBrother.Domain.Entities;

namespace BigBrother.Domain.ProviderInterfaces;

public interface IScoreProvider {
    public Task<IEnumerable<Score>> GetSessionsScoresAsync(int sessionId, CancellationToken cancellationToken);

    public Task AddScoreAsync(Score score, CancellationToken cancellationToken);
}
