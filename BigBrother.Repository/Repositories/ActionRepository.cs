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

    public async Task AddActionAsync(IdeAction action, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        var entity = new IdeActionEntity 
        {
            ActionType = action.ActionType,
            Message = action.Message,
            DetectTime = action.DetectTime,
            SessionId = action.SessionId,
            UserId = action.UserId
        };
        await context.IdeActions.AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserIdeActionsDistribution>> GetUserIdeActionDistributionsInSessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        var usersActions = new List<UserIdeActionsDistribution>();

        using var context = _contextFactory.GetContext();

        var session = await context.Sessions
            .Include(x => x.Group)
            .ThenInclude(x => x!.Users)
            .AsNoTracking()
            .FirstAsync(x => x.Id == sessionId);
        var userIds = session.Group!.Users.Select(x => x.Id);

        var actionTypes = Enum.GetValues<IdeActionType>();

        foreach (var userId in userIds) 
        {
            var actionCounts = new Dictionary<IdeActionType, int>();
            foreach (var actionType in actionTypes) 
            {
                actionCounts[actionType] = await context.IdeActions
                    .AsNoTracking()
                    .Where(x => x.SessionId == sessionId && x.UserId == userId && x.ActionType == actionType)
                    .CountAsync(cancellationToken);
            }
            var userActions = new UserIdeActionsDistribution 
            {
                IdeActionsDistribution = actionCounts,
                UserId = userId
            };

            usersActions.Add(userActions);
        }
        
        return usersActions;
    }

    public async Task<IEnumerable<IdeAction>> GetIdeActionsInSessionByUserAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        return await context.IdeActions
            .AsNoTracking()
            .Where(x => x.SessionId == sessionId && x.UserId == userId)
            .OrderBy(x => x.DetectTime)
            .Select(x => new IdeAction {
                Id = x.Id,
                ActionType = x.ActionType,
                Message = x.Message,
                DetectTime = x.DetectTime,
                SessionId = x.SessionId,
                UserId = x.UserId
            })
            .ToListAsync(cancellationToken);
    }
}
