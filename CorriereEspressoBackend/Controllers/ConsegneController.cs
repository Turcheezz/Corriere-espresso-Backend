using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorriereEspressoBackend.DBContext;
using CorriereEspressoBackend.Dto;
using CorriereEspressoBackend.Models;

namespace CorriereEspressoBackend.Controllers;

[ApiController]
[Route("api/consegne")]
[Authorize]
public class ConsegneController : ControllerBase
{
    private readonly AppDbContext _context;

    private static readonly string[] StatiValidi = { "Da ritirare", "In deposito", "In consegna", "Consegnata", "In giacenza" };

    public ConsegneController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? clienteId, [FromQuery] string? stato)
    {
        var query = _context.Consegne
            .Include(c => c.Cliente)
            .AsQueryable();

        if (clienteId.HasValue)
            query = query.Where(c => c.ClienteId == clienteId.Value);

        if (!string.IsNullOrWhiteSpace(stato))
            query = query.Where(c => c.Stato == stato);

        var consegne = await query
            .OrderByDescending(c => c.DataRitiro)
            .ToListAsync();

        return Ok(consegne);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var consegna = await _context.Consegne
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.ConsegnaId == id);

        if (consegna == null)
            return NotFound("Consegna non trovata");

        return Ok(consegna);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ConsegnaRequestDto dto)
    {
        if (dto.ClienteId <= 0)
            return BadRequest("Cliente non valido");

        var cliente = await _context.Clienti.FindAsync(dto.ClienteId);
        if (cliente == null)
            return BadRequest("Cliente non esistente");

        var stato = !string.IsNullOrWhiteSpace(dto.Stato) && StatiValidi.Contains(dto.Stato)
            ? dto.Stato : "Da ritirare";

        var chiave = $"TRK-2026-{DateTime.UtcNow.Ticks % 100000:D6}";

        var consegna = new Consegna
        {
            ClienteId = dto.ClienteId,
            DataRitiro = dto.DataRitiro,
            Stato = stato,
            ChiaveConsegna = chiave
        };

        _context.Consegne.Add(consegna);
        await _context.SaveChangesAsync();

        return Ok(consegna);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ConsegnaRequestDto dto)
    {
        var consegna = await _context.Consegne.FindAsync(id);
        if (consegna == null)
            return NotFound("Consegna non trovata");

        if (dto.ClienteId <= 0)
            return BadRequest("Cliente non valido");

        var cliente = await _context.Clienti.FindAsync(dto.ClienteId);
        if (cliente == null)
            return BadRequest("Cliente non esistente");

        consegna.ClienteId = dto.ClienteId;
        consegna.DataRitiro = dto.DataRitiro;

        if (!string.IsNullOrWhiteSpace(dto.Stato) && StatiValidi.Contains(dto.Stato))
            consegna.Stato = dto.Stato;

        await _context.SaveChangesAsync();

        return Ok(consegna);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var consegna = await _context.Consegne.FindAsync(id);
        if (consegna == null)
            return NotFound("Consegna non trovata");

        if (consegna.Stato == "Consegnata")
            return BadRequest("Impossibile eliminare: consegna già consegnata");

        _context.Consegne.Remove(consegna);
        await _context.SaveChangesAsync();

        return Ok("Consegna eliminata");
    }

    [HttpPut("{id}/stato")]
    public async Task<IActionResult> AggiornaStato(int id, AggiornaStatoDto dto)
    {
        var consegna = await _context.Consegne.FindAsync(id);
        if (consegna == null)
            return NotFound("Consegna non trovata");

        if (!StatiValidi.Contains(dto.Stato))
            return BadRequest($"Stato non valido. Valori ammessi: {string.Join(", ", StatiValidi)}");

        consegna.Stato = dto.Stato;

        if (dto.Stato == "Consegnata")
        {
            if (consegna.DataConsegna == null)
                consegna.DataConsegna = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(consegna);
    }
}
