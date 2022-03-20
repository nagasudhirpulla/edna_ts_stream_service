using EdnaTsStreamService;
using EdnaTsStreamService.Infra.Data;
using EdnaTsStreamService.Infra.Edna;
using EdnaTsStreamService.Infra.Measurements;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<MeasService>();
        services.AddSingleton<DataFetcher>();
        services.AddSingleton<DataClient>();
        services.AddHostedService<DataStreamWorker>();
    })
    .Build();

await host.RunAsync();
