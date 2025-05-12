using System.Text.Json.Serialization;
using BigBrother.Domain.Entities.Enums;

namespace BigBrother.WebApp.Dtos;

public sealed class IdeActionDto 
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("type")]
    public IdeActionType Type {get; set;}

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("detectTime")]
    public DateTime DetectTime { get; set; }

    [JsonPropertyName("sessionId")]
    public int SessionId { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }
}
