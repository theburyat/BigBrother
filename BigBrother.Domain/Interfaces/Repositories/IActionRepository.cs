using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Repositories;

public interface IActionRepository {
    Task AddActionAsync(IdeAction action, CancellationToken cancellationToken);

    Task<IEnumerable<UserActions>> GetSessionUsersActionsAsync(int sessionId, CancellationToken cancellationToken);

    Task<IEnumerable<IdeAction>> GetSessionUserActionsAsync(int sessionId, int userId, CancellationToken cancellationToken);
}
