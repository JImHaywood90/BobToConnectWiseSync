namespace Dto.ConnectWise;

public class ConnectWiseScheduleEntryPayload
{
    public object member { get; set; } = null!;
    public object scheduleType { get; set; } = null!;
    public DateTime start { get; set; }
    public DateTime end { get; set; }
    public string? notes { get; set; }
}
