using System.Text.Json.Serialization;

namespace Dto.Bob;
public sealed record TimeOffCreatedEvent
{
    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    [JsonPropertyName("triggeredBy")]
    public required string TriggeredBy { get; set; }
    [JsonPropertyName("triggeredAt")]
    public DateTime TriggeredAt { get; set; }
    [JsonPropertyName("version")]
    public required string Version { get; set; }
    [JsonPropertyName("data")]
    public required TimeOffCreatedEventData Data { get; set; }
}
