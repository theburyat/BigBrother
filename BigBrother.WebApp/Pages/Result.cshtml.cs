using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;

namespace BigBrother.WebApp.Pages;

public class ResultModel : BasePageModel
{
    private readonly IScoreProvider _scoreProvider;
    private readonly IUserProvider _userProvider;
    private readonly IActionProvider _actionProvider;

    public Score? Score { get; set; }
    public new User? User { get; set; }
    public IEnumerable<IdeAction>? IdeActions { get; set; }


    public ResultModel(IScoreProvider scoreProvider, IUserProvider userProvider, IActionProvider actionProvider)
    {
        _scoreProvider = scoreProvider;
        _userProvider = userProvider;
        _actionProvider = actionProvider;
    }

    public async Task OnGetAsync(int sessionId, int userId)
    {
        var cancellationToken = HttpContext.RequestAborted;

        await SafeInvokeAsync(() => GetPageInfoAsync(sessionId, userId, cancellationToken));
    }
    
    private async Task GetPageInfoAsync(int sessionId, int userId, CancellationToken cancellationToken)
    {
        Score = await _scoreProvider.GetScoreAsync(sessionId, userId, cancellationToken);
        User = await _userProvider.GetUserAsync(userId, cancellationToken);
        IdeActions = await _actionProvider.GetIdeActionsInSessionByUserAsync(sessionId, userId, cancellationToken);
    }
}
