using AutoMapper;
using BigBrother.Interfaces;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.Controllers;

[ApiController]
[Route("[controller]")]
public class ExamsController: ControllerBase
{
    private readonly IExamService _examService;
    private readonly IMapper _mapper;

    public ExamsController(IExamService examService, IMapper mapper)
    {
        _examService = examService;
        _mapper = mapper;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<Guid> SaveExamAsync(
        [FromQuery] string userName,
        [FromQuery] string userGroup,
        [FromForm] IFormFile logFile, 
        CancellationToken cancellationToken)
    {
        var user = new CreateUserModel
        {
            Name = userName,
            Group = userGroup
        };
        
        var examId = await _examService.SaveExamLogAsync(user, logFile, cancellationToken);
        return examId;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<ExamModel> GetExamAsync(Guid id, CancellationToken cancellationToken)
    {
        var exam = await _examService.GetExamAsync(id, cancellationToken);
        return _mapper.Map<ExamModel>(exam);
    }
    
    [HttpGet("{id}/log")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<string> GetExamLogAsync(Guid id, CancellationToken cancellationToken)
    {
        var examLog = await _examService.GetExamLogAsync(id, cancellationToken);
        return examLog;
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<Guid> DeleteExamAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = await _examService.DeleteExamAsync(id, cancellationToken);
        return userId;
    }
}
