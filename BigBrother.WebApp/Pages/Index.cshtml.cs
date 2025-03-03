using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigBrother.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IGroupProvider _groupProvider;
    private readonly Initializer _initializer;

    public IEnumerable<Group>? Groups { get; private set; }

    public IndexModel(IGroupProvider groupProvider, Initializer initializer)
    {
        _groupProvider = groupProvider;
        _initializer = initializer;
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

    public async Task OnPostInitialize()
    {
        var cancellationToken = HttpContext.RequestAborted;

        await _initializer.InitializeAsync(cancellationToken);
        await GetPageInfoAsync(cancellationToken);
    }

    private async Task GetPageInfoAsync(CancellationToken cancellationToken)
    {
        Groups = await _groupProvider.GetGroupsAsync(cancellationToken);
    }
}
