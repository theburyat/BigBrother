using System.Text.Json.Serialization;
using BigBrother.Domain.Entities.Enums;

namespace BigBrother.WebApp.Dtos;

public class ErrorDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("code")]
    public ErrorCode Code { get; init; }
    
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}