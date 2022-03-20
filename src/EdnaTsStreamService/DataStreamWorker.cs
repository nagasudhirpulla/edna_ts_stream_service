using Cronos;
using EdnaTsStreamService.Infra.Data;
using EdnaTsStreamService.Infra.Edna;
using EdnaTsStreamService.Infra.Measurements;

namespace EdnaTsStreamService;

public class DataStreamWorker : BackgroundService
{
    private readonly DataFetcher _fetcher;
    private readonly string _periodicity;
    private readonly List<Meas> _measList;
    private readonly DataClient _dataClient;
    private readonly ILogger<DataStreamWorker> _logger;

    public DataStreamWorker(ILogger<DataStreamWorker> logger, DataFetcher fetcher, IConfiguration configuration, MeasService measService, DataClient dataClient)
    {
        _logger = logger;
        _fetcher = fetcher;
        _dataClient = dataClient;
        _periodicity = configuration.GetValue<string>("FetchConfig:Periodicity");
        _measList = measService.GetMeasList();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await WaitForNextSchedule(_periodicity);
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // fetch data of all measurements
            List<EdnaRtVal> measData = new();
            foreach (var m in _measList)
            {
                try
                {
                    EdnaRtVal val = _fetcher.GetMeasRtVal(m.MeasId);
                    measData.Add(val);
                    _logger.LogInformation("{value}", val.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error while fetching real time data of edna measurement {mId}, {mName}, {msg}", m.MeasId, m.MeasName, ex.Message);
                }
            }

            // push all data into data store
            foreach (var mVal in measData)
            {
                _ = await _dataClient.InsertRecord(mVal.Ts, mVal.Id, mVal.Val, stoppingToken);
            }
        }
    }

    private async Task WaitForNextSchedule(string cronExpression)
    {
        var parsedExp = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
        var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
        var occurenceTime = parsedExp.GetNextOccurrence(currentUtcTime);

        var delay = occurenceTime!.Value - currentUtcTime;
        _logger.LogInformation("The run is delayed for {delay}. Current time: {time}", delay, DateTimeOffset.Now);

        await Task.Delay(delay);
    }
}