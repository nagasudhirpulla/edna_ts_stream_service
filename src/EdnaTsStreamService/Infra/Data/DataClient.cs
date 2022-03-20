using EdnaTsStreamService.Configs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace EdnaTsStreamService.Infra.Data;

public class DataClient
{
    private readonly DataClientConfig _dataClientConfig;
    private readonly ILogger<DataClient> _logger;
    public DataClient(IConfiguration configuration, ILogger<DataClient> logger)
    {
        _logger = logger;
        _dataClientConfig = new();
        configuration.GetSection("DataClient").Bind(_dataClientConfig);
    }
    public async Task<bool> InsertRecord(DateTime ts, string mId, double val, CancellationToken cancellationToken = new CancellationToken())
    {
        // https://github.com/dotnet-labs/ApiAuthDemo/blob/master/BasicAuthApiConsumer/Program.cs
        try
        {
            // create http client
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                AuthenticationSchemes.Basic.ToString(),
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_dataClientConfig.ApiKey))
                );

            // call the data insertion API and get response
            var response = await httpClient.PostAsJsonAsync(_dataClientConfig.InsertUrl, new DataInsCommand { Ts = ts.ToString("yyyy-MM-dd HH:mm:ss"), MeasId = mId, Val = val }, cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync(cancellationToken) ?? "";

            // check the data insertion result
            if (result == "true")
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while inserting record into data store, {msg}", ex.Message);
            return false;
        }

        return false;
    }
}
