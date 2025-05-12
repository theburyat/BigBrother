using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Services;

namespace BigBrother.WebApp.Pages;

public class IndexModel : BasePageModel
{
    private readonly IGroupProvider _groupProvider;
    private readonly InitializeService _initializeService;

    public IEnumerable<Group>? Groups { get; private set; }

    public IndexModel(IGroupProvider groupProvider, InitializeService initializeService)
    {
        _groupProvider = groupProvider;
        _initializeService = initializeService;
    }

    public async Task OnGetAsync()
    {
        var cancellationToken = HttpContext.RequestAborted;
        
        await SafeInvokeAsync(() => GetPageInfoAsync(cancellationToken));
    }

    public async Task OnPostAsync(string name)
    {
        var cancellationToken = HttpContext.RequestAborted;
        
        await SafeInvokeAsync(async () =>
        {
            await _groupProvider.CreateGroupAsync(name, cancellationToken);
            await GetPageInfoAsync(cancellationToken);
        });
    }
    
    public async Task OnPostDeleteAsync(int id)
    {
        var cancellationToken = HttpContext.RequestAborted;

        await SafeInvokeAsync(async () =>
        {
            await _groupProvider.DeleteGroupAsync(id, cancellationToken);
            await GetPageInfoAsync(cancellationToken);
        });
    }

    public async Task OnPostInitialize()
    {
        var cancellationToken = HttpContext.RequestAborted;

        await SafeInvokeAsync(async () =>
        {
            await _initializeService.InitializeAsync(cancellationToken);
            await GetPageInfoAsync(cancellationToken);
        });
    }

    private async Task GetPageInfoAsync(CancellationToken cancellationToken)
    {
        Groups = await _groupProvider.GetGroupsAsync(cancellationToken);
    }
}
