namespace CorriereEspressoBackend.Dto;

public class RegisterDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfermaPassword { get; set; } = string.Empty;
}
