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
        await using var context = _contextFactory.GetContext();

        var entity = new IdeActionEntity 
        {
            Type = action.Type,
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
        var result = new List<UserIdeActionsDistribution>();
        
        var actionTypes = Enum.GetValues<IdeActionType>();

        await using var context = _contextFactory.GetContext();
        var session = await context.Sessions
            .Include(x => x.Group)
            .ThenInclude(x => x!.Users)
            .AsNoTracking()
            .FirstAsync(x => x.Id == sessionId, cancellationToken);
        
        var usersInSession = session.Group!.Users.Select(x => x.Id);
        foreach (var userId in usersInSession) 
        {
            var actionDistribution = new Dictionary<IdeActionType, int>();
            foreach (var actionType in actionTypes) 
            {
                actionDistribution[actionType] = await context.IdeActions
                    .AsNoTracking()
                    .Where(x => x.SessionId == sessionId && x.UserId == userId && x.Type == actionType)
                    .CountAsync(cancellationToken);
            }
            var userActions = new UserIdeActionsDistribution 
            {
                IdeActionsDistribution = actionDistribution,
                UserId = userId
            };

            result.Add(userActions);
        }
        
        return result;
    }

    public async Task<IEnumerable<IdeAction>> GetIdeActionsInSessionByUserAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        return await context.IdeActions
            .AsNoTracking()
            .Where(x => x.SessionId == sessionId && x.UserId == userId)
            .OrderBy(x => x.DetectTime)
            .Select(x => new IdeAction {
                Id = x.Id,
                Type = x.Type,
                Message = x.Message,
                DetectTime = x.DetectTime,
                SessionId = x.SessionId,
                UserId = x.UserId
            })
            .ToListAsync(cancellationToken);
    }
}
