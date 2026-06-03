using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorriereEspressoBackend.DBContext;
using CorriereEspressoBackend.Dto;

namespace CorriereEspressoBackend.Controllers;

[ApiController]
[Route("api/tracking")]
public class TrackingController : ControllerBase
{
    private readonly AppDbContext _context;

    public TrackingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Tracking(TrackingRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ChiaveConsegna))
            return BadRequest("Chiave di tracking obbligatoria");

        var consegna = await _context.Consegne
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c =>
                c.ChiaveConsegna == dto.ChiaveConsegna &&
                c.DataRitiro.Date == dto.DataRitiro.Date);

        if (consegna == null)
            return NotFound("Nessuna consegna trovata con i dati forniti");

        return Ok(new
        {
            consegna.Stato,
            consegna.DataRitiro,
            consegna.DataConsegna
        });
    }
}
