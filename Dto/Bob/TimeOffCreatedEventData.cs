using System.Text.Json.Serialization;

namespace Dto.Bob;
public sealed record TimeOffCreatedEventData
{
    [JsonPropertyName("timeoffRequestId")]
    public int TimeoffRequestId { get; set; }
    [JsonPropertyName("employeeId")]
    public required string EmployeeId { get; set; }
    [JsonPropertyName("getApi")]
    public required string GetApi { get; set; }
}
