using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Entities.Exceptions;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Services;

namespace BigBrother.Domain.Services;

public sealed class ConnectionService: IConnectionService
{
    private readonly IUserProvider _userProvider;
    private readonly ISessionProvider _sessionProvider;

    public ConnectionService(IUserProvider userProvider, ISessionProvider sessionProvider)
    {
        _userProvider = userProvider;
        _sessionProvider = sessionProvider;
    }

    public async Task<int> ConnectAsync(string username, int sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessionProvider.GetSessionAsync(sessionId, cancellationToken);
        if (!session.IsRunning())
        {
            throw new BadRequestException(ErrorCode.SessionIsNotActive, $"Session '{sessionId}' is not active");
        }

        if (!await _userProvider.IsUserExistAsync(username, session.GroupId, cancellationToken))
        {
            return await _userProvider.CreateUserAsync(username, session.GroupId, cancellationToken);
        }
        
        var user = await _userProvider.GetUserAsync(username, session.GroupId, cancellationToken);
        return user.Id;
    }
}