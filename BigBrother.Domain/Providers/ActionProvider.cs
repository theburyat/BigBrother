using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;

namespace BigBrother.Domain.Providers;

public sealed class ActionProvider : IActionProvider
{
    private readonly IActionRepository _repository;
    private readonly ISessionProvider _sessionProvider;
    private readonly IUserProvider _userProvider;

    public ActionProvider(IActionRepository repository, ISessionProvider sessionProvider, IUserProvider userProvider)
    {
        _repository = repository;
        _sessionProvider = sessionProvider;
        _userProvider = userProvider;
    }

    public async Task AddActionAsync(@Action action, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(action);

        await _sessionProvider.EnsureSessionExistAsync(action.SessionId, cancellationToken);
        await _userProvider.EnsureUserExistAsync(action.UserId, cancellationToken);
        // to do check if user and session have same group id

        await _repository.AddActionAsync(action, cancellationToken);
    }

    public async Task<IEnumerable<UserActions>> GetSessionUsersActionsAsync(int sessionId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);

        return await _repository.GetSessionUsersActionsAsync(sessionId, cancellationToken);
    }

    public async Task<IEnumerable<Action>> GetSessionUserActionsAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);
        await _userProvider.EnsureUserExistAsync(userId, cancellationToken);
        // to do check if user and session have same group id

        return await _repository.GetSessionUserActionsAsync(sessionId, userId, cancellationToken);
    }
}
