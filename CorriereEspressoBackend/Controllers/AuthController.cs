using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorriereEspressoBackend.DBContext;
using CorriereEspressoBackend.Dto;
using CorriereEspressoBackend.Models;
using CorriereEspressoBackend.Services;

namespace CorriereEspressoBackend.Controllers;

[ApiController]
[Route("api/operatori/")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            return BadRequest("Nome obbligatorio");
        if (string.IsNullOrWhiteSpace(dto.Cognome))
            return BadRequest("Cognome obbligatorio");
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Email obbligatoria");
        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Password obbligatoria");
        if (dto.Password != dto.ConfermaPassword)
            return BadRequest("Password e conferma password non coincidono");

        var exists = await _context.Operatori.AnyAsync(x => x.Email == dto.Email);
        if (exists)
            return BadRequest("Email già utilizzata");

        var user = new Operatore
        {
            Nome = dto.Nome,
            Cognome = dto.Cognome,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Operatori.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { user.OperatoreId, user.Nome, user.Cognome, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Email e password obbligatorie");

        var operatore = await _context.Operatori
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (operatore == null || !BCrypt.Net.BCrypt.Verify(dto.Password, operatore.Password))
            return Unauthorized("Email o password errate");

        var token = _jwtService.Generate(operatore);

        return Ok(new { token });
    }
}
