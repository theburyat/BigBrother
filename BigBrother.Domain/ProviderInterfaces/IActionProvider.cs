using BigBrother.Domain.Entities;

namespace BigBrother.Domain.ProviderInterfaces;

public interface IActionProvider {
    Task AddActionAsync(IdeAction action, CancellationToken cancellationToken);

    Task<IEnumerable<IdeAction>> GetUserSessionActionsAsync(int sessionId, int userId, CancellationToken cancellationToken);
}
