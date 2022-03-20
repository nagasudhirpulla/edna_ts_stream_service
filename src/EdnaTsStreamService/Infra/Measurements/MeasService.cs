using Microsoft.VisualBasic.FileIO;

namespace EdnaTsStreamService.Infra.Measurements;

public class MeasService
{
    private readonly string _measListFilePath;
    private readonly ILogger<MeasService> _logger;
    public MeasService(IConfiguration configuration, ILogger<MeasService> logger)
    {
        _measListFilePath = configuration.GetValue<string>("MeasListFilePath");
        _logger = logger;
    }

    public List<Meas> GetMeasList()
    {
        // https://stackoverflow.com/questions/5282999/reading-csv-file-and-storing-values-into-an-array
        // https://stackoverflow.com/questions/3507498/reading-csv-files-using-c-sharp
        List<Meas> measList = new();
        try
        {
            using TextFieldParser parser = new(_measListFilePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                // Processing csv row
                string[]? fields = parser.ReadFields();
                if (fields != null && fields.Length >= 2)
                {
                    measList.Add(new Meas() { MeasId = fields[0], MeasName = fields[1] });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while reading measurements, {msg}", ex.Message);
        }

        return measList;
    }

}
