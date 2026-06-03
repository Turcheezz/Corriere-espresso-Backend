namespace CorriereEspressoBackend.Dto;

public class ClienteRequestDto
{
    public string Nominativo { get; set; } = string.Empty;
    public string Via { get; set; } = string.Empty;
    public string Comune { get; set; } = string.Empty;
    public string? Provincia { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Note { get; set; }
}
