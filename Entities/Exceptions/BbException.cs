using System.Text.Json.Serialization;
using Entities.Enums;

namespace Entities.Exceptions;

public class BbException: Exception
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorCode ErrorCode { get; }

    public BbException(ErrorCode errorCode)
    {
        ErrorCode = errorCode;
    }

    public BbException(ErrorCode errorCode, string message): base(message)
    {
        ErrorCode = errorCode;
    }

    public BbException(ErrorCode errorCode, string message, Exception inner): base(message, inner)
    {
        ErrorCode = errorCode;
    }
}
