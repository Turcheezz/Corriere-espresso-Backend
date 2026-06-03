namespace CorriereEspressoBackend.Dto;

public class StatisticheResponseDto
{
    public DateTime Dal { get; set; }
    public DateTime Al { get; set; }
    public string? Stato { get; set; }
    public int NumeroConsegne { get; set; }
    public double? TempoMedioConsegnaOre { get; set; }
}
