using System.Runtime.InteropServices;
using System.Text.Json;

const string dataDirectory = @"C:\ProgramData\ArioTjarat\Agent";
const string deviceIdFile = "device-id.txt";
const string activityQueueFile = "activity-queue.jsonl";

Directory.CreateDirectory(dataDirectory);

var deviceIdPath = Path.Combine(dataDirectory, deviceIdFile);
var queuePath = Path.Combine(dataDirectory, activityQueueFile);

string deviceId;

if (File.Exists(deviceIdPath))
{
    deviceId = File.ReadAllText(deviceIdPath).Trim();

    if (string.IsNullOrWhiteSpace(deviceId))
    {
        deviceId = $"WIN-{Guid.NewGuid():N}";
        File.WriteAllText(deviceIdPath, deviceId);
    }
}
else
{
    deviceId = $"WIN-{Guid.NewGuid():N}";
    File.WriteAllText(deviceIdPath, deviceId);
}

while (true)
{
    try
    {
        var idleSeconds = GetIdleSeconds();

        var record = new
        {
            DeviceId = deviceId,
            Timestamp = DateTimeOffset.UtcNow,
            IdleSeconds = idleSeconds,
            State = idleSeconds >= 60 ? "idle" : "active",
            MachineName = Environment.MachineName,
            UserName = Environment.UserName
        };

        await File.AppendAllTextAsync(
            queuePath,
            JsonSerializer.Serialize(record) + Environment.NewLine
        );
    }
    catch
    {
    }

    await Task.Delay(TimeSpan.FromSeconds(30));
}

static long GetIdleSeconds()
{
    var info = new LASTINPUTINFO
    {
        cbSize = (uint)Marshal.SizeOf<LASTINPUTINFO>()
    };

    if (!GetLastInputInfo(ref info))
        return 0;

    var idleMilliseconds = (ulong)Environment.TickCount64 - info.dwTime;

    return (long)(idleMilliseconds / 1000);
}

[StructLayout(LayoutKind.Sequential)]
struct LASTINPUTINFO
{
    public uint cbSize;
    public uint dwTime;
}

[DllImport("user32.dll")]
static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
