using BigBrother.Interfaces;
using Entities.Domain;
using Entities.Enums;
using Entities.Exceptions;

namespace BigBrother.Services;

public class LogFileParserService: ILogFileParserService
{
    public async Task ParseLogFileAsync(IFormFile file, Exam exam)
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
                    case UserAction.PasteFromUnknownSource:
                        exam.PasteFromUnknownSourceCount += 1;
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
                    case UserAction.Run:
                        exam.RunningCount += 1;
                        break;
                    case UserAction.Build:
                        exam.BuildingCount += 1;
                        break;
                }
                continue;
            }
            
            throw new BbException(ErrorCode.INVALID_ACTION, $"Invalid action: {actionTypeString}");
        }
    }
}
