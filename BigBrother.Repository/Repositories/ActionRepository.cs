global using Action = BigBrother.Domain.Entities.Action;

using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Interfaces.Repositories;
using BigBrother.Repository.Context.Factory;
using BigBrother.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Repositories;

public class ActionRepository : IActionRepository
{
    private readonly IContextFactory _contextFactory;

    public ActionRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddActionAsync(Action action, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        var entity = new ActionEntity 
        {
            ActionType = action.ActionType,
            DetectTime = action.DetectTime,
            SessionId = action.SessionId,
            UserId = action.UserId,
            Message = action.Message
        };
        await context.Actions.AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserActions>> GetSessionUsersActionsAsync(int sessionId, CancellationToken cancellationToken)
    {
        var usersActions = new List<UserActions>();

        using var context = _contextFactory.GetContext();

        var session = await context.Sessions.Include(x => x.Group).ThenInclude(x => x!.Users).AsNoTracking()
            .FirstAsync(x => x.Id == sessionId);
        var userIds = session.Group!.Users.Select(x => x.Id);

        var actionTypes = Enum.GetValues<ActionType>();

        foreach (var userId in userIds) 
        {
            var actionCounts = new Dictionary<ActionType, int>();
            foreach (var actionType in actionTypes) 
            {
                var count = await context.Actions.AsNoTracking()
                    .Where(x => x.SessionId == sessionId && x.UserId == userId && x.ActionType == actionType)
                    .CountAsync(cancellationToken);
                
                actionCounts[actionType] = count;
            }
            var userActions = new UserActions 
            {
                UserId = userId,
                Actions = actionCounts
            };

            usersActions.Add(userActions);
        }
        
        return usersActions;
    }

    public async Task<IEnumerable<Action>> GetSessionUserActionsAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        return await context.Actions.AsNoTracking()
            .Where(x => x.SessionId == sessionId && x.UserId == userId)
            .OrderBy(x => x.DetectTime)
            .Select(x => new Action {
                Id = x.Id,
                ActionType = x.ActionType,
                DetectTime = x.DetectTime,
                SessionId = x.SessionId,
                UserId = x.UserId,
                Message = x.Message
            })
            .ToListAsync(cancellationToken);
    }
}
