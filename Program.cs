using DisplayCountWatcherService;

IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.UseWindowsService();

hostBuilder.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();
    services.AddSingleton<PIDStorage>();
});

hostBuilder.Build().Run();

