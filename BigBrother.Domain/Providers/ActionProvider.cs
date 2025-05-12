using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Entities.Exceptions;
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

    public async Task AddIdeActionAsync(IdeAction ideAction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ideAction);
        
        await _userProvider.EnsureUserExistAsync(ideAction.UserId, cancellationToken);
        
        var session = await _sessionProvider.GetSessionAsync(ideAction.SessionId, cancellationToken);
        if (!session.IsRunning())
        {
            throw new BadRequestException(ErrorCode.SessionIsNotActive, $"Session {session.Id} is not active");
        }

        await _repository.AddActionAsync(ideAction, cancellationToken);
    }

    public async Task<IEnumerable<UserIdeActionsDistribution>> GetUserIdeActionDistributionsInSessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);

        return await _repository.GetUserIdeActionDistributionsInSessionAsync(sessionId, cancellationToken);
    }

    public async Task<IEnumerable<IdeAction>> GetIdeActionsInSessionByUserAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        await _sessionProvider.EnsureSessionExistAsync(sessionId, cancellationToken);
        await _userProvider.EnsureUserExistAsync(userId, cancellationToken);
        // to do check if user and session have same group id

        return await _repository.GetIdeActionsInSessionByUserAsync(sessionId, userId, cancellationToken);
    }
}
