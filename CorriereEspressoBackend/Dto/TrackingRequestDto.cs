namespace CorriereEspressoBackend.Dto;

public class TrackingRequestDto
{
    public string ChiaveConsegna { get; set; } = string.Empty;
    public DateTime DataRitiro { get; set; }
}
