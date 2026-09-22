using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

public sealed class AgentWorker : BackgroundService
{
    private const string DataDirectory = @"C:\ProgramData\ArioTjarat\Agent";
    private const string DeviceIdFile = "device-id.txt";
    private const string ActivityQueueFile = "activity-queue.jsonl";

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Directory.CreateDirectory(DataDirectory);

        var deviceId = GetOrCreateDeviceId();
        var queuePath = Path.Combine(DataDirectory, ActivityQueueFile);

        while (!stoppingToken.IsCancellationRequested)
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
                    JsonSerializer.Serialize(record) + Environment.NewLine,
                    stoppingToken);
            }
            catch
            {
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private static string GetOrCreateDeviceId()
    {
        var path = Path.Combine(DataDirectory, DeviceIdFile);

        if (File.Exists(path))
        {
            var existing = File.ReadAllText(path).Trim();

            if (!string.IsNullOrWhiteSpace(existing))
                return existing;
        }

        var deviceId = $"WIN-{Guid.NewGuid():N}";
        File.WriteAllText(path, deviceId);
        return deviceId;
    }

    private static long GetIdleSeconds()
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
}
