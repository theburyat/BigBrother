using BigBrother.Interfaces;
using Entities.Domain;
using Entities.Enums;
using Entities.Exceptions;
using Entities.Models;
using Repository.Interfaces;

namespace BigBrother.Services;

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    private readonly IUserService _userService;
    private readonly ILogFileParserService _logFileParserService;
    private readonly ILogger<ExamService> _logger;

    public ExamService(
        IExamRepository examRepository,
        IUserService userService,
        ILogFileParserService logFileParserService,
        ILogger<ExamService> logger)
    {
        _userService = userService;
        _examRepository = examRepository;
        _logFileParserService = logFileParserService;
        _logger = logger;
    }

    public async Task<Guid> SaveExamLogAsync(
        CreateUserModel userModel,
        IFormFile logfile,
        CancellationToken cancellationToken)
    {
        Guid userId = await _userService.GetUserIdByNameWithCreationIfNotExistAsync(userModel, cancellationToken);
        
        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = DateTime.UtcNow
        };

        await _logFileParserService.ParseLogFileAsync(logfile, exam);

        return await _examRepository.CreateExamAsync(exam, cancellationToken);
    }

    public async Task<Exam> GetExamAsync(Guid examId, CancellationToken cancellationToken)
    {
        var exam = await _examRepository.GetExamAsync(examId, cancellationToken);
        if (exam is null)
        {
            throw new BbException(ErrorCode.EXAM_NOT_FOUND, $"Exam with id {examId} not found");
        }

        return exam;
    }

    public async Task<string> GetExamLogAsync(Guid examId, CancellationToken cancellationToken)
    {
        var exam = await _examRepository.GetExamAsync(examId, cancellationToken);
        if (exam is null)
        {
            throw new BbException(ErrorCode.EXAM_NOT_FOUND, $"Exam with id {examId} not found");
        }
        
        return exam.ActionsLog;
    }

    public Exam GetUserExamAtDate(User user, DateTime dateTime)
    {
        var userExams = GetUserExams(user.Id).ToArray();
        if (userExams.Length == 0)
        {
            throw new BbException(ErrorCode.TOO_FEW_EXAMS, 
                $"User {user.Name} from group {user.Group} with id {user.Id} has no exams");
        }

        var desiredExam = userExams.LastOrDefault(x => x.Date.Date == dateTime.Date);
        if (desiredExam is null)
        {
            throw new BbException(ErrorCode.EXAM_NOT_FOUND,
                $"User {user.Name} from group {user.Group} with id {user.Id} has no exams at the date {dateTime}");
        }

        return desiredExam;
    }

    private IReadOnlyCollection<Exam> GetUserExams(Guid userId)
    {
        return _examRepository.GetUserExams(userId);
    }

    public async Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken)
    {
       var deletedExamId =  await _examRepository.DeleteExamAsync(examId, cancellationToken);
       if (deletedExamId == Guid.Empty)
       {
           throw new BbException(ErrorCode.EXAM_NOT_FOUND, $"User with id {deletedExamId} not found");
       }

       return deletedExamId;
    }
}
