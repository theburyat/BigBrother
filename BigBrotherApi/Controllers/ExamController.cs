using BigBrother.Interfaces;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.Controllers;

[ApiController]
[Route("[controller]")]
public class ExamController: ControllerBase
{
    private readonly IExamService _examService;

    public ExamController(IExamService examService)
    {
        _examService = examService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<Guid> SaveExamLogAsync(
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
    
    [HttpGet("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<UserModel> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _examService.GetUserAsync(id, cancellationToken);
        return user;
    }
    
    [HttpGet("users/find")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<UserModel> GetUserAsync(
        [FromQuery] string userName, 
        [FromQuery] string userGroup,
        CancellationToken cancellationToken)
    {
        var user = await _examService.GetUserByNameAsync(userName, userGroup, cancellationToken);
        return user;
    }

    [HttpGet("exams/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<ExamModel> GetExamAsync(Guid id, CancellationToken cancellationToken)
    {
        var exam = await _examService.GetExamAsync(id, cancellationToken);
        return exam;
    }
    
    [HttpGet("exams/log/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<string> GetExamLogAsync(Guid id, CancellationToken cancellationToken)
    {
        var examLog = await _examService.GetExamLogAsync(id, cancellationToken);
        return examLog;
    }

    [HttpDelete("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<Guid> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = await _examService.DeleteUserAsync(id, cancellationToken);
        return userId;
    }
    
    [HttpDelete("exams/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<Guid> DeleteExamAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = await _examService.DeleteExamAsync(id, cancellationToken);
        return userId;
    }
}