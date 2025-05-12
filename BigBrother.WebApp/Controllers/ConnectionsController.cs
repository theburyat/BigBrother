using BigBrother.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ConnectionsController: ControllerBase
{
    private readonly IConnectionService _connectionService;
    
    public ConnectionsController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    [HttpPost("sessions/{sessionId:int}")]
    public Task<int> ConnectAsync(int sessionId, [FromQuery] string username, CancellationToken cancellationToken)
    {
        return _connectionService.ConnectAsync(username, sessionId, cancellationToken);
    }
}