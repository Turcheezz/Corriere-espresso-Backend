namespace CorriereEspressoBackend.Models;

public class Consegna
{
    public int ConsegnaId { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataRitiro { get; set; }
    public DateTime? DataConsegna { get; set; }
    public string Stato { get; set; } = "Da ritirare";
    public string ChiaveConsegna { get; set; } = string.Empty;

    public Cliente Cliente { get; set; } = null!;
}
