using BigBrother.Domain.Entities;

namespace BigBrother.Domain.RepositoryInterfaces;

public interface IActionRepository {
    Task AddActionAsync(IdeAction action, CancellationToken cancellationToken);

    Task<IEnumerable<IdeAction>> GetUserSessionActionsAsync(int sessionId, int userId, CancellationToken cancellationToken);
}
