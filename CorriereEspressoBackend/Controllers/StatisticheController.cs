using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorriereEspressoBackend.DBContext;
using CorriereEspressoBackend.Dto;

namespace CorriereEspressoBackend.Controllers;

[ApiController]
[Route("api/statistiche")]
[Authorize]
public class StatisticheController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatisticheController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("consegne")]
    public async Task<IActionResult> GetStatistiche(
        [FromQuery] DateTime? dal,
        [FromQuery] DateTime? al,
        [FromQuery] string? stato)
    {
        var dataDal = dal ?? DateTime.MinValue;
        var dataAl = al ?? DateTime.MaxValue;

        var query = _context.Consegne
            .Where(c => c.DataRitiro >= dataDal && c.DataRitiro <= dataAl)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(stato))
            query = query.Where(c => c.Stato == stato);

        var consegne = await query.ToListAsync();

        var numeroConsegne = consegne.Count;
        double? tempoMedioOre = null;

        var consegnate = consegne.Where(c => c.Stato == "Consegnata" && c.DataConsegna.HasValue).ToList();
        if (consegnate.Any())
        {
            tempoMedioOre = consegnate
                .Average(c => (c.DataConsegna!.Value - c.DataRitiro).TotalHours);
        }

        var result = new StatisticheResponseDto
        {
            Dal = dataDal,
            Al = dataAl,
            Stato = stato,
            NumeroConsegne = numeroConsegne,
            TempoMedioConsegnaOre = Math.Round(tempoMedioOre ?? 0, 1)
        };

        return Ok(new[] { result });
    }
}
