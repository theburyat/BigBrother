using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface IActionRepository
{
    Task AddActionAsync(IdeAction action, CancellationToken cancellationToken);

    Task<IEnumerable<UserIdeActionsDistribution>> GetUserIdeActionDistributionsInSessionAsync(int sessionId, CancellationToken cancellationToken);

    Task<IEnumerable<IdeAction>> GetIdeActionsInSessionByUserAsync(int sessionId, int userId, CancellationToken cancellationToken);
}
