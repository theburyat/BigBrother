using System.Text;
using BigBrother.Interfaces;
using Entities.Domain;
using Entities.Enums;
using Entities.Exceptions;

namespace BigBrother.Services;

public class DetectionService: IDetectionService
{
    private readonly IExamService _examService;
    private readonly IUserService _userService;
    private readonly IGenerationFromScratchDetectionService _generationFromScratchDetectionService;
    
    public DetectionService(
        IExamService examService,
        IUserService userService,
        IGenerationFromScratchDetectionService generationFromScratchDetectionService)
    {
        _examService = examService;
        _userService = userService;
        _generationFromScratchDetectionService = generationFromScratchDetectionService;
    }


    public async Task<string> RunAnalysisAsync(string dateLine, string group, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(dateLine, out var date))
        {
            throw new BbException(ErrorCode.INVALID_DATE, $"Invalid date: {dateLine}");
        }
        
        var exams = new List<Exam>();
        
        var usersForDetection = _userService.GetUsersFromGroup(group);
        foreach (var user in usersForDetection)
        {
            var userExamAtSelectedDate = _examService.GetUserExamAtDate(user, date);
            if (userExamAtSelectedDate is not null)
            {
                exams.Add(userExamAtSelectedDate);
            }
        }
        
        var generationFromScratchDetectionResult = 
            await _generationFromScratchDetectionService.DetectGenerationFromScratchAsync(exams, cancellationToken); 
        
        var stringBuilder = new StringBuilder();

        var normalizedGenerationFromScratchDetectionResult =
            await GetHumanReadableGenerationFromScratchDetectionResultAsync(generationFromScratchDetectionResult, cancellationToken);

        stringBuilder.Append(normalizedGenerationFromScratchDetectionResult);
        
        return stringBuilder.ToString();
    }

    private async Task<StringBuilder> GetHumanReadableGenerationFromScratchDetectionResultAsync(IDictionary<Guid, double> detectionResult, CancellationToken cancellationToken)
    {
        var stringBuilder = new StringBuilder("Generation From Scratch detection result").AppendLine();
        var sortedResult = detectionResult.OrderBy(x => x.Value);

        foreach (var entry in sortedResult)
        {
            var userName = await GetUserNameByExamIdAsync(entry.Key, cancellationToken);

            stringBuilder.AppendLine($"Total outlier score for user {userName} is {Math.Round(entry.Value, 2)}");
        }

        return stringBuilder;
    }

    private async Task<string> GetUserNameByExamIdAsync(Guid examId, CancellationToken cancellationToken)
    {
        var exam = await _examService.GetExamAsync(examId, cancellationToken);
        var user = await _userService.GetUserAsync(exam.UserId, cancellationToken);

        return user.Name;
    }
}