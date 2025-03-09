using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Interfaces.Providers;

namespace BigBrother.Domain.Services;

public class Initializer
{
    private readonly IGroupProvider _groupProvider;
    private readonly ISessionProvider _sessionProvider;
    private readonly IUserProvider _userProvider;
    private readonly IActionProvider _actionProvider;

    public Initializer(IGroupProvider groupProvider, ISessionProvider sessionProvider, IUserProvider userProvider, IActionProvider actionProvider)
    {
        _groupProvider = groupProvider;
        _sessionProvider = sessionProvider;
        _userProvider = userProvider;
        _actionProvider = actionProvider;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var random = new Random();
        
        var groupName = "initialized";
        var groupId = await _groupProvider.CreateGroupAsync(groupName, cancellationToken);

        var sessionId = await _sessionProvider.CreateSessionAsync(groupId, cancellationToken);

        var actionTypes = Enum.GetValues<IdeActionType>();
        for (var i = 0; i < 30; i++)
        {
            var userName = $"user{i}";
            var userId = await _userProvider.CreateUserAsync(userName, groupId, cancellationToken);

            var tasks = new List<Task>();
            for (var j = 0; j < 100; j++)
            {
                var action = new IdeAction
                {
                    ActionType = actionTypes[random.Next(actionTypes.Length)],
                    DetectTime = DateTime.UtcNow,
                    SessionId = sessionId,
                    UserId = userId,
                    Message = random.Next(10) < 2 ? "test message" : string.Empty 
                };
                tasks.Add(_actionProvider.AddIdeActionAsync(action, cancellationToken));
            }
            await Task.WhenAll(tasks);
        }
    }
}