using BigBrother.Domain.Entities.Enums;

namespace BigBrother.Domain.Entities.Exceptions;

public sealed class BadRequestException: Exception
{
    public BadRequestException(ErrorCode code, string message): base(message)
    {
        Code = code;
    }
    
    public ErrorCode Code { get; init; }
}
