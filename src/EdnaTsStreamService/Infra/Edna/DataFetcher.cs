namespace EdnaTsStreamService.Infra.Edna;

using EdnaTsStreamService.Configs;
using InStep.eDNA.EzDNAApiNet;
public class DataFetcher
{
    private readonly FetchConfig _fetchConfig;
    public DataFetcher(IConfiguration configuration)
    {
        _fetchConfig = new();
        configuration.GetSection("FetchConfig").Bind(_fetchConfig);
    }

    public EdnaRtVal GetMeasRtVal(string pntId)
    {
        if (_fetchConfig.IsDummy)
        {
            // check if dummy data is to be sent as per configuration
            return new EdnaRtVal { Ts = DateTime.Now, Id = pntId, Val = new Random().NextInt64(10, 100) };
        }

        int nret = RealTime.DNAGetRTAll(pntId, out double dval, out DateTime timestamp, out _, out _, out _);
        if (nret == 0)
        {
            var realVal = new EdnaRtVal { Ts = timestamp, Id = pntId, Val = dval };
            return realVal;
        }
        throw new Exception("Unable to retrieve real time edna measurement Id data");
    }
}
