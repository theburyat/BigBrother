using BigBrother.Domain.Entities;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.WebApp.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IdeActionsController : ControllerBase
{
    private readonly IActionProvider _actionProvider;

    public IdeActionsController(IActionProvider actionProvider)
    {
        _actionProvider = actionProvider;
    }
    
    [HttpPost]
    public Task AddActionAsync([FromBody] IdeActionDto ideActionDto, CancellationToken cancellationToken) 
    {
        var ideAction = new IdeAction 
        {
            Id = Guid.NewGuid(),
            Type = ideActionDto.Type,
            Message = ideActionDto.Message,
            DetectTime = ideActionDto.DetectTime,
            UserId = ideActionDto.UserId,
            SessionId = ideActionDto.SessionId
        };

        return _actionProvider.AddIdeActionAsync(ideAction, cancellationToken);
    }
}
