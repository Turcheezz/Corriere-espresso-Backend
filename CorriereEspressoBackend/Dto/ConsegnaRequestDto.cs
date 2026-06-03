namespace CorriereEspressoBackend.Dto;

public class ConsegnaRequestDto
{
    public int ClienteId { get; set; }
    public DateTime DataRitiro { get; set; }
    public string? Stato { get; set; }
}
