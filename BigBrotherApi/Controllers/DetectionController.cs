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
    public async Task<string> RunAnalysisAsync(CancellationToken cancellationToken)
    {
        var x = await _copyAndModifyDetectionService.DetectCopyAndModify("353", DateTime.Today);
        var xx = await _generationFromScratchDetectionService.DetectGenerationFromScratchAsync("353", DateTime.Today, cancellationToken); 
        
        var y = new StringBuilder();

        foreach (var z in x.Keys)
        {
            y.AppendLine($"correlation between {z.Item1} and {z.Item2} is: {x[z]}");
        }

        return y.ToString();
    }
}
