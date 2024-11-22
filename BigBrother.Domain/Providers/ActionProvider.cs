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
        await ValidateActionAsync(action, cancellationToken);
        await _repository.AddActionAsync(action, cancellationToken);
    }

    public async Task<IEnumerable<UserActions>> GetSessionUsersActionsAsync(int sessionId, CancellationToken cancellationToken)
    {
        if (!await _sessionProvider.IsSessionExistAsync(sessionId, cancellationToken)) 
        {
            throw new Exception();
        }
        return await _repository.GetSessionUsersActionsAsync(sessionId, cancellationToken);
    }

    public async Task<IEnumerable<Action>> GetSessionUserActionsAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        await ValidateActionParametersAsync(sessionId, userId, cancellationToken);
        return await _repository.GetSessionUserActionsAsync(sessionId, userId, cancellationToken);
    }

    private async Task ValidateActionAsync(Action action, CancellationToken cancellationToken) 
    {
        ArgumentNullException.ThrowIfNull(action);
        await ValidateActionParametersAsync(action.SessionId, action.UserId, cancellationToken);
    }

    private async Task ValidateActionParametersAsync(int sessionId, int userId, CancellationToken cancellationToken) 
    {
        if (!await _sessionProvider.IsSessionExistAsync(sessionId, cancellationToken)) 
        {
            throw new Exception();
        }
        if (!await _userProvider.IsUserExistAsync(userId, cancellationToken)) 
        {
            throw new Exception();
        }

        // TODO check if session group id = user group id
    }
}
