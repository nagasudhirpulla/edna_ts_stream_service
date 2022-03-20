namespace EdnaTsStreamService.Infra.Edna;

public record EdnaRtVal
{
    public DateTime Ts { get; init; }
    public string Id { get; init; }
    public double Val { get; init; }
};
