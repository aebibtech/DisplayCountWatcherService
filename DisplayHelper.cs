using System.Runtime.InteropServices;

namespace DisplayCountWatcherService;

public static class DisplayHelper
{
    private static int _monitorCount = 0;

    public static bool IsExternalMonitorConnected()
    {
        _monitorCount = 0;
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);
        return _monitorCount > 1;
    }

    private static bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
    {
        _monitorCount++;
        return true;
    }

    private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }
}

public class PIDStorage
{
    public const int DefaultPid = -1;
    public int ProcessId { get; set; } = DefaultPid;
}
