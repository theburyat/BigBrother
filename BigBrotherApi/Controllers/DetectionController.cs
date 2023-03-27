using BigBrother.Interfaces;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.Controllers;

[ApiController]
[Route("[controller]")]
public class DetectionController
{
    private readonly IDetectionService _detectionService;

    public DetectionController(IDetectionService detectionService)
    {
        _detectionService = detectionService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public Task<string> RunAnalysisAsync([FromBody] StartAnalysisModel startAnalysisModel, CancellationToken cancellationToken)
    {
        return _detectionService.RunAnalysisAsync(startAnalysisModel.Date, startAnalysisModel.Group, cancellationToken);
    }
}
