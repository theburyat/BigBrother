using Entities.Domain;

namespace BigBrother.Interfaces;

public interface ILogFileParserService
{
    Task ParseLogFileAsync(IFormFile file, Exam exam);
}