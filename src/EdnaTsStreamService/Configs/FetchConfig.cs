namespace EdnaTsStreamService.Configs;

public class FetchConfig
{
    public string Periodicity { get; set; } = "@hourly";
    public bool IsDummy { get; set; } = true;
}
