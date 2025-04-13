using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;

namespace DisplayCountWatcherService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly PIDStorage _storage;

    public Worker(ILogger<Worker> logger, IConfiguration config, PIDStorage storage)
    {
        _logger = logger;
        _config = config;
        _storage = storage;
        if (OperatingSystem.IsWindows())
        {
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        }
    }

    private void OnDisplaySettingsChanged(object sender, EventArgs e)
    {
        string? appPath = _config.GetValue<string>("AppPath");
        string? args = _config.GetValue<string>("Args");
        string? workingDirectory = _config.GetValue<string>("WorkingDirectory");
        _logger.LogInformation("Display settings changed at: {time}", DateTimeOffset.Now);

        if (IsExternalMonitorConnected())
        {
            _logger.LogInformation("External display detected. Running {ex}...", appPath);
            if (File.Exists(appPath))
            {
                try
                {
                    Process? process = Process.Start(new ProcessStartInfo()
                    {
                        Arguments = args,
                        FileName = appPath,
                        WorkingDirectory = workingDirectory ?? string.Empty
                    });
                    _storage.ProcessId = (process?.Id).GetValueOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error starting process for executable. {ex}", ex);
                }
            }
            else
            {
                _logger.LogError("Executable file {a} does not exist.", appPath);
            }
        }
        else
        {
            _logger.LogInformation("External display removed. Killing executable.");
            try
            {
                Process? executableProcess = Process.GetProcessById(_storage.ProcessId);
                executableProcess?.Kill();
                _logger.LogInformation("Successfully killed executable with process ID {p}.", executableProcess?.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error killing executable: {ex}", ex);
            }
            finally
            {
                _storage.ProcessId = PIDStorage.DefaultPid;
            }
        }
    }

    private static bool IsExternalMonitorConnected()
    {
        return DisplayHelper.IsExternalMonitorConnected();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Display count watcher service is running.\n\tExecutable file path: {ex}\n\tArguments: {a}.",
            _config.GetValue<string>("AppPath"), _config.GetValue<string>("Args"));
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        base.Dispose();
        if (OperatingSystem.IsWindows())
        {
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        }
        GC.SuppressFinalize(this);
    }
}

