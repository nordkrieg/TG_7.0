using System.Diagnostics;
namespace Guard;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    public Worker(ILogger<Worker> logger) { _logger = logger; }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            const string appPath = @"Schedule.exe";
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
            process.Start();
            await process.WaitForExitAsync(stoppingToken);
            if (process.HasExited)
            {
                _logger.LogInformation("Application has exited. Restarting at: {time}", DateTimeOffset.Now);
                process.Start();
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}