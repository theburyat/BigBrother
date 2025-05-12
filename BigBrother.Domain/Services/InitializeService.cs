using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Entities.Exceptions;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Services;

namespace BigBrother.Domain.Services;

public class InitializeService: IInitializeService
{
    private readonly IGroupProvider _groupProvider;
    private readonly ISessionProvider _sessionProvider;
    private readonly IUserProvider _userProvider;
    private readonly IActionProvider _actionProvider;
    
    private readonly DateTime _sessionStartDate = DateTime.UtcNow.AddHours(-3);
    private readonly DateTime _sessionEndDate = DateTime.UtcNow;

    private const string Symbols = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
    private const string CopiedText = "copied text";
    private const string PastedText = "pasted text";
    
    private readonly Random _random = new();

    public InitializeService(IGroupProvider groupProvider, ISessionProvider sessionProvider, IUserProvider userProvider, IActionProvider actionProvider)
    {
        _groupProvider = groupProvider;
        _sessionProvider = sessionProvider;
        _userProvider = userProvider;
        _actionProvider = actionProvider;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var groups = await _groupProvider.GetGroupsAsync(cancellationToken);
        if (groups.Any())
        {
            throw new BadRequestException(ErrorCode.GroupsIsNotEmpty, "Could not create test data because there are existing groups");
        }

        var groupName = $"test data - {Guid.NewGuid()}";
        var groupId = await _groupProvider.CreateGroupAsync(groupName, cancellationToken);

        var sessionId = await _sessionProvider.CreateSessionAsync(groupId, cancellationToken);
        await _sessionProvider.StartSessionAsync(sessionId, cancellationToken);

        await InitializeNormalGroupAsync(groupId, sessionId, cancellationToken);
        await InitializeAnomalyGroupAsync(groupId, sessionId, cancellationToken);
        
        await _sessionProvider.StopSessionAsync(sessionId, cancellationToken);
    }

    private async Task InitializeNormalGroupAsync(int groupId, int sessionId, CancellationToken cancellationToken)
    {
        for (var i = 0; i < 28; i++)
        {
            var username = $"normal user - {i}";
            var userId = await _userProvider.CreateUserAsync(username, groupId, cancellationToken);
            
            var tasks = new List<Task>();
            
            for (var j = 0; j < 150; j++)
            {
                var action = new IdeAction
                {
                    Type = IdeActionType.Type,
                    DetectTime = GenerateDetectTime(),
                    SessionId = sessionId,
                    UserId = userId,
                    Message = Symbols[_random.Next(0, Symbols.Length)].ToString()
                };
                tasks.Add(_actionProvider.AddIdeActionAsync(action, cancellationToken));
            }

        for (var j = 0; j < 50; j++)
            {
                var action = new IdeAction
                {
                    Type = IdeActionType.Delete,
                    DetectTime = GenerateDetectTime(),
                    SessionId = sessionId,
                    UserId = userId,
                    Message = Symbols[_random.Next(0, Symbols.Length)].ToString()
                };
                tasks.Add(_actionProvider.AddIdeActionAsync(action, cancellationToken));
            }
            
            await Task.WhenAll(tasks);
        }
    }

    private async Task InitializeAnomalyGroupAsync(int groupId, int sessionId, CancellationToken cancellationToken)
    {
        for (var i = 0; i < 2; i++)
        {
            var username = $"anomaly user - {i}";
            var userId = await _userProvider.CreateUserAsync(username, groupId, cancellationToken);
            
            var tasks = new List<Task>();
            
            for (var j = 0; j < 50; j++)
            {
                var action = new IdeAction
                {
                    Type = IdeActionType.Type,
                    DetectTime = GenerateDetectTime(),
                    SessionId = sessionId,
                    UserId = userId,
                    Message = Symbols[_random.Next(0, Symbols.Length)].ToString()
                };
                tasks.Add(_actionProvider.AddIdeActionAsync(action, cancellationToken));
            }
            for (var j = 0; j < 10; j++)
            {
                var action = new IdeAction
                {
                    Type = IdeActionType.Delete,
                    DetectTime = GenerateDetectTime(),
                    SessionId = sessionId,
                    UserId = userId,
                    Message = Symbols[_random.Next(0, Symbols.Length)].ToString()
                };
                tasks.Add(_actionProvider.AddIdeActionAsync(action, cancellationToken));
            }
            
            for (var j = 0; j < 50; j++)
            {
                var action = new IdeAction
                {
                    Type = IdeActionType.Copy,
                    DetectTime = GenerateDetectTime(),
                    SessionId = sessionId,
                    UserId = userId,
                    Message = CopiedText
                };
                tasks.Add(_actionProvider.AddIdeActionAsync(action, cancellationToken));
            }
            
            for (var j = 0; j < 50; j++)
            {
                var action = new IdeAction
                {
                    Type = IdeActionType.Paste,
                    DetectTime = GenerateDetectTime(),
                    SessionId = sessionId,
                    UserId = userId,
                    Message = PastedText
                };
                tasks.Add(_actionProvider.AddIdeActionAsync(action, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }
    }

    private DateTime GenerateDetectTime()
    {
        var sessionLength = _sessionEndDate - _sessionStartDate;
        return _sessionStartDate + new TimeSpan(0, _random.Next(0, (int)sessionLength.TotalMinutes), 0);
    }
}