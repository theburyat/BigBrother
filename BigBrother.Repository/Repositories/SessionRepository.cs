using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Repositories;
using BigBrother.Repository.Context.Factory;
using BigBrother.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly IContextFactory _contextFactory;

    public SessionRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var sessionEntity = new SessionEntity { GroupId = groupId };
        await context.Sessions.AddAsync(sessionEntity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return sessionEntity.Id;
    }

     public async Task<IEnumerable<Session>> GetSessionsInGroupAsync(int groupId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        return await context.Sessions
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .Select(x => new Session 
            {
                Id = x.Id,
                GroupId = x.GroupId,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Session?> GetSessionAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var session = await context.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return session == null
            ? null
            : new Session
            {
                Id = session.Id,
                GroupId = session.GroupId,
                StartDate = session.StartDate,
                EndDate = session.EndDate
            };
    }

    public async Task DeleteSessionAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        await context.Sessions
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task StartSessionAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        await context.Sessions
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.StartDate, y => DateTime.UtcNow), cancellationToken);
    }

    public async Task StopSessionAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        await context.Sessions
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.EndDate, y => DateTime.UtcNow), cancellationToken);
    }

    public async Task<bool> IsSessionExistAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        return await context.Sessions
            .AsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
}
