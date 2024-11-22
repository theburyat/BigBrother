using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Providers;

public interface IActionProvider {
    Task AddActionAsync(Action action, CancellationToken cancellationToken);

    Task<IEnumerable<UserActions>> GetSessionUsersActionsAsync(int sessionId, CancellationToken cancellationToken);

    Task<IEnumerable<Action>> GetSessionUserActionsAsync(int sessionId, int userId, CancellationToken cancellationToken);
}
