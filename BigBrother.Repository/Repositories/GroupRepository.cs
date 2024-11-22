using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Repositories;
using BigBrother.Repository.Context.Factory;
using BigBrother.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBrother.Repository.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly IContextFactory _contextFactory;

    public GroupRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Group>> GetGroupsAsync(CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        return await context.Groups.AsNoTracking().Select(x => new Group { Id = x.Id, Name = x.Name }).ToListAsync(cancellationToken);
    }

    public async Task<int> CreateGroupAsync(string name, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();

        var group = new GroupEntity { Name = name };
        await context.Groups.AddAsync(group, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return group.Id;
    }

    public Task DeleteGroupAsync(int id, CancellationToken cancellationToken)
    {
         using var context = _contextFactory.GetContext();
         
         return context.Groups.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public Task<bool> IsGroupExistAsync(int id, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();
        
        return context.Groups.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> IsGroupExistAsync(string name, CancellationToken cancellationToken)
    {
        using var context = _contextFactory.GetContext();
        
        return context.Groups.AsNoTracking().AnyAsync(x => string.Equals(x.Name, name, StringComparison.Ordinal), cancellationToken);
    }
}
