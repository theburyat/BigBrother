using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigBrother.WebApp.Pages;

public class SessionModel : PageModel
{
    private readonly ISessionProvider _sessionProvider;
    private readonly IScoreProvider _scoreProvider;
    private readonly IUserProvider _userProvider;
    private readonly IAnalysisService _analysisService;

    public Session? Session { get; set; }

    public IEnumerable<Score>? Scores { get; set; }

    public IEnumerable<User>? Users { get; set; }

    public SessionModel(ISessionProvider sessionProvider, IScoreProvider scoreProvider, IUserProvider userProvider, IAnalysisService analysisService)
    {
        _sessionProvider = sessionProvider;
        _scoreProvider = scoreProvider;
        _userProvider = userProvider;
        _analysisService = analysisService;
    }

    public async Task OnGetAsync(int id)
    {
        var cancellationToken = HttpContext.RequestAborted;
        
        await GetPageInfoAsync(id, cancellationToken);
    }

    public async Task OnPostStartAsync(int id) 
    {
        var cancellationToken = HttpContext.RequestAborted;

        await _sessionProvider.StartSessionAsync(id, cancellationToken);
        await GetPageInfoAsync(id, cancellationToken);
    }

    public async Task OnPostStopAsync(int id) 
    {
        var cancellationToken = HttpContext.RequestAborted;

        await _sessionProvider.StopSessionAsync(id, cancellationToken);
        await GetPageInfoAsync(id, cancellationToken);
    }

    public async Task OnPostAnalyzeAsync(int id) 
    {
        var cancellationToken = HttpContext.RequestAborted;

        await _analysisService.RunAnalysisAsync(id, cancellationToken);
        await GetPageInfoAsync(id, cancellationToken);
    }
    
    private async Task GetPageInfoAsync(int id, CancellationToken cancellationToken)
    {
        Session = await _sessionProvider.GetSessionAsync(id, cancellationToken);
        Users = await _userProvider.GetUsersBySessionAsync(id, cancellationToken);
        
        var scores = await _scoreProvider.GetScoresBySessionAsync(id, cancellationToken);
        Scores = scores.OrderBy(x => x.Rating);
    }
}
