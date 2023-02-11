using BigBrother.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.Controllers;

[ApiController]
[Route("[controller]")]
public class ExamController: ControllerBase
{
    private IExamService _examService;

    public ExamController(IExamService examService)
    {
        _examService = examService;
    }
}