using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Repositories;
using BigBrother.Repository.Context.Factory;
using BigBrother.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Repositories;

public class ScoreRepository : IScoreRepository
{
    private readonly IContextFactory _contextFactory;

    public ScoreRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddScoreAsync(Score score, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var entity = new ScoreEntity 
        {
            Rating = score.Rating,
            SessionId = score.SessionId,
            UserId = score.UserId
        };
        await context.Scores.AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Score>> GetScoresInSessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        return await context.Scores
            .AsNoTracking()
            .Where(x => x.SessionId == sessionId)
            .Select(x => new Score 
            {
                Rating = x.Rating,
                SessionId = x.UserId,
                UserId = x.UserId
            })
            .ToListAsync();
    }

    public async Task<Score?> GetScoreAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.GetContext();

        var entity = await context.Scores
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.UserId == userId, cancellationToken);
        
        if (entity == null)
        {
            return null;
        }

        return new Score 
        {
            SessionId = entity.SessionId,
            UserId = entity.UserId,
            Rating = entity.Rating
        };
    }
}
