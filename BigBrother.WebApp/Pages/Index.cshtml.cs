using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigBrother.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IGroupProvider _groupProvider;

    public IEnumerable<Group>? Groups { get; private set; }

    public IndexModel(IGroupProvider groupProvider)
    {
        _groupProvider = groupProvider;
    }

    public async Task OnGetAsync()
    {
        var cancellationToken = HttpContext.RequestAborted;

        await GetPageInfoAsync(cancellationToken);
    }

    public async Task OnPostAsync(string name)
    {
        var cancellationToken = HttpContext.RequestAborted;

        await _groupProvider.CreateGroupAsync(name, cancellationToken);
        await GetPageInfoAsync(cancellationToken);
    }
    
    public async Task OnPostDeleteAsync(int id)
    {
        var cancellationToken = HttpContext.RequestAborted;

        await _groupProvider.DeleteGroupAsync(id, cancellationToken);
        await GetPageInfoAsync(cancellationToken);
    }
    
    private async Task GetPageInfoAsync(CancellationToken cancellationToken)
    {
        Groups = await _groupProvider.GetGroupsAsync(cancellationToken);
    }
}
