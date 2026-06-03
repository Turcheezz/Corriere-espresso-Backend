using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorriereEspressoBackend.DBContext;
using CorriereEspressoBackend.Dto;
using CorriereEspressoBackend.Models;

namespace CorriereEspressoBackend.Controllers;

[ApiController]
[Route("api/clienti")]
[Authorize]
public class ClientiController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientiController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clienti = await _context.Clienti
            .OrderBy(c => c.Nominativo)
            .ToListAsync();
        return Ok(clienti);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _context.Clienti.FindAsync(id);
        if (cliente == null)
            return NotFound("Cliente non trovato");
        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ClienteRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nominativo))
            return BadRequest("Nominativo obbligatorio");
        if (string.IsNullOrWhiteSpace(dto.Via))
            return BadRequest("Indirizzo obbligatorio");
        if (string.IsNullOrWhiteSpace(dto.Comune))
            return BadRequest("Comune obbligatorio");

        var cliente = new Cliente
        {
            Nominativo = dto.Nominativo,
            Via = dto.Via,
            Comune = dto.Comune,
            Provincia = dto.Provincia,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Note = dto.Note
        };

        _context.Clienti.Add(cliente);
        await _context.SaveChangesAsync();

        return Ok(cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ClienteRequestDto dto)
    {
        var cliente = await _context.Clienti.FindAsync(id);
        if (cliente == null)
            return NotFound("Cliente non trovato");

        if (string.IsNullOrWhiteSpace(dto.Nominativo))
            return BadRequest("Nominativo obbligatorio");
        if (string.IsNullOrWhiteSpace(dto.Via))
            return BadRequest("Indirizzo obbligatorio");
        if (string.IsNullOrWhiteSpace(dto.Comune))
            return BadRequest("Comune obbligatorio");

        cliente.Nominativo = dto.Nominativo;
        cliente.Via = dto.Via;
        cliente.Comune = dto.Comune;
        cliente.Provincia = dto.Provincia;
        cliente.Telefono = dto.Telefono;
        cliente.Email = dto.Email;
        cliente.Note = dto.Note;

        await _context.SaveChangesAsync();

        return Ok(cliente);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clienti
            .Include(c => c.Consegne)
            .FirstOrDefaultAsync(c => c.ClienteId == id);

        if (cliente == null)
            return NotFound("Cliente non trovato");

        if (cliente.Consegne.Any())
            return BadRequest("Impossibile eliminare: il cliente ha consegne associate");

        _context.Clienti.Remove(cliente);
        await _context.SaveChangesAsync();

        return Ok("Cliente eliminato");
    }
}
