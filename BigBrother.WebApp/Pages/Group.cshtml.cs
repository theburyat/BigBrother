using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigBrother.WebApp.Pages;

public class GroupModel : PageModel 
{
    private readonly IGroupProvider _groupProvider;
    private readonly ISessionProvider _sessionProvider;

    public GroupModel(IGroupProvider groupProvider, ISessionProvider sessionProvider)
    {
        _groupProvider = groupProvider;
        _sessionProvider = sessionProvider;
    }

    public Group? Group { get; private set; }
    public IEnumerable<Session>? Sessions { get; private set; }

    public async Task OnGetAsync(int id)
    {
        var cancellationToken = HttpContext.RequestAborted;

        await GetPageInfoAsync(id, cancellationToken);
    }

    public async Task OnPostAsync(int id)
    {
        var cancellationToken = HttpContext.RequestAborted;
        
        await _sessionProvider.CreateSessionAsync(id, cancellationToken);
        await GetPageInfoAsync(id, cancellationToken);
    }
    
    public async Task OnPostDeleteAsync(int id, int sessionId)
    {
        var cancellationToken = HttpContext.RequestAborted;

        await _sessionProvider.DeleteSessionAsync(sessionId, cancellationToken);
        await GetPageInfoAsync(id, cancellationToken);
    }

    private async Task GetPageInfoAsync(int id, CancellationToken cancellationToken)
    {
        Group = await _groupProvider.GetGroupAsync(id, cancellationToken);
        Sessions = await _sessionProvider.GetSessionsByGroupAsync(id, cancellationToken);
    }
}
