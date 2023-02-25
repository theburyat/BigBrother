using AutoMapper;
using BigBrother.Interfaces;
using Entities.Domain;
using Entities.Enums;
using Entities.Exceptions;
using Entities.Models;
using Repository.Interfaces;

namespace BigBrother.Services;

public class ExamService : IExamService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;
    //private readonly ILogger<ExamService> _logger;

    public ExamService(IRepository repository, IMapper mapper/*, ILogger<ExamService> logger*/)
    {
        _repository = repository;
        _mapper = mapper;
        //_logger = logger;
    }

    public async Task<Guid> SaveExamLogAsync(
        CreateUserModel userModel,
        IFormFile logfile,
        CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId(userModel, cancellationToken);
        
        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = DateTime.UtcNow
        };

        await ParseLogFileAsync(logfile, exam);

        return await _repository.CreateExamAsync(exam, cancellationToken);
    }

    private async Task<Guid> GetUserId(CreateUserModel userModel, CancellationToken cancellationToken)
    {
        return _repository.UserWithNameExists(userModel.Name, userModel.Group)
            ? (await _repository.GetUserByNameAsync(userModel.Name, userModel.Group, cancellationToken)).Id
            : await CreateUserAsync(userModel, cancellationToken);
    }

    private async Task<Guid> CreateUserAsync(CreateUserModel userModel, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(userModel);
        user.Id = Guid.NewGuid();

        return await _repository.CreateUserAsync(user, cancellationToken);
    }

    private async Task ParseLogFileAsync(IFormFile file, Exam exam)
    {
        var fileLines = new List<string>();
        var fileStream = file.OpenReadStream();
        using (var reader = new StreamReader(fileStream))
        {
            while (reader.Peek() >= 0)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line) && reader.Peek() >= 0)
                {
                    throw new BbException(ErrorCode.INVALID_LOG_FILE, "Empty line");
                }

                fileLines.Add(line!);
            }

            fileStream.Position = 0;
            
            exam.ActionsLog = await reader.ReadToEndAsync();
        }

        if (fileLines.Count == 0)
        {
            throw new BbException(ErrorCode.INVALID_LOG_FILE, "Invalid log file");
        }
        
        ParseLogFileContent(fileLines, exam);
    }

    private void ParseLogFileContent(List<string> lines, Exam exam)
    {
        foreach (var line in lines)
        {
            string actionTypeString;
            try
            {
                actionTypeString = line.Split(" ")[1];
            }
            catch (Exception ex)
            {
                throw new BbException(ErrorCode.INVALID_LOG_FILE, $"Invalid line: {line}", ex);
            }
            
            if (Enum.TryParse(typeof(UserAction), actionTypeString, false, out var actionType))
            {
                switch (actionType)
                {
                    case UserAction.Backspace: 
                        exam.BackspaceCount += 1; 
                        break;
                    case UserAction.Copy:
                        exam.CopyCount += 1;
                        break;
                    case UserAction.Cut:
                        exam.CutCount += 1;
                        break;
                    case UserAction.Delete:
                        exam.DeleteCount += 1;
                        break;
                    case UserAction.Enter:
                        exam.EnterCount += 1;
                        break;
                    case UserAction.Select:
                        exam.SelectCount += 1;
                        break;
                    case UserAction.Paste:
                        exam.PasteCount += 1;
                        break;
                    case UserAction.Type:
                        exam.TypeCount += 1;
                        break;
                    case UserAction.MoveCaretDown:
                        exam.MoveCaretDownCount += 1;
                        break;
                    case UserAction.MoveCaretLeft:
                        exam.MoveCaretLeftCount += 1;
                        break;
                    case UserAction.MoveCaretRight:
                        exam.MoveCaretRightCount += 1;
                        break;
                    case UserAction.MoveCaretUp:
                        exam.MoveCaretUpCount += 1;
                        break;
                    case UserAction.CompleteCode:
                        exam.CodeCompletionCount += 1;
                        break;
                    case UserAction.Running:
                        exam.RunningCount += 1;
                        break;
                    case UserAction.Building:
                        exam.BuildingCount += 1;
                        break;
                }
                continue;
            }
            
            throw new BbException(ErrorCode.INVALID_ACTION, $"Invalid action: {actionTypeString}");
        }
    }

    public async Task<UserModel> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new BbException(ErrorCode.USER_NOT_FOUND, $"User with id {userId} not found");
        }

        return _mapper.Map<UserModel>(user);
    }

    public async Task<UserModel> GetUserByNameAsync(string userName, string userGroup, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserByNameAsync(userName, userGroup, cancellationToken);
        if (user is null)
        {
            throw new BbException(ErrorCode.USER_NOT_FOUND, $"User with name {userName} from group {userGroup} not found");
        }
        
        return _mapper.Map<UserModel>(user);
    }

    public async Task<ExamModel> GetExamAsync(Guid examId, CancellationToken cancellationToken)
    {
        var exam = await _repository.GetExamAsync(examId, cancellationToken);
        if (exam is null)
        {
            throw new BbException(ErrorCode.EXAM_NOT_FOUND, $"Exam with id {examId} not found");
        }

        return _mapper.Map<ExamModel>(exam);
    }

    public async Task<string> GetExamLogAsync(Guid examId, CancellationToken cancellationToken)
    {
        var examLog = await _repository.GetExamLogAsync(examId, cancellationToken);
        
        return examLog ?? throw new BbException(ErrorCode.EXAM_NOT_FOUND, $"Exam with id {examId} not found");
    }

    public async Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var deletedUserId = await _repository.DeleteUserAsync(userId, cancellationToken);
        if (deletedUserId == Guid.Empty)
        {
            throw new BbException(ErrorCode.USER_NOT_FOUND, $"User with id {userId} not found");
        }

        return deletedUserId;
    }

    public async Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken)
    {
       var deletedExamId =  await _repository.DeleteExamAsync(examId, cancellationToken);
       if (deletedExamId == Guid.Empty)
       {
           throw new BbException(ErrorCode.EXAM_NOT_FOUND, $"User with id {deletedExamId} not found");
       }

       return deletedExamId;
    }
}