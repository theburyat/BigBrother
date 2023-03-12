using System.Text;
using BigBrother.Interfaces;
using Entities.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.Controllers;

[ApiController]
[Route("[controller]")]
public class DetectionController
{
    private readonly ICopyAndModifyDetectionService _copyAndModifyDetectionService;
    private readonly IGenerationFromScratchDetectionService _generationFromScratchDetectionService;

    public DetectionController(
        ICopyAndModifyDetectionService copyAndModifyDetectionService, 
        IGenerationFromScratchDetectionService generationFromScratchDetectionService)
    {
        _copyAndModifyDetectionService = copyAndModifyDetectionService;
        _generationFromScratchDetectionService = generationFromScratchDetectionService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public Task<string> RunAnalysisAsync(CancellationToken cancellationToken)
    {
        var x = _copyAndModifyDetectionService.DetectCopyAndModify("353", DateTime.Today);
        var xx = _generationFromScratchDetectionService.DetectGenerationFromScratchAsync("353", DateTime.Today, cancellationToken); 
        
        var y = new StringBuilder();

        foreach (var z in x.Keys)
        {
            y.AppendLine($"correlation between {z.Item1} and {z.Item2} is: {x[z]}");
        }

        return Task.FromResult(y.ToString());
    }
}
