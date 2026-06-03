using Microsoft.EntityFrameworkCore;
using CorriereEspressoBackend.Models;

namespace CorriereEspressoBackend.DBContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Operatore> Operatori { get; set; }
    public DbSet<Cliente> Clienti { get; set; }
    public DbSet<Consegna> Consegne { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Consegna>()
            .HasOne(c => c.Cliente)
            .WithMany(cl => cl.Consegne)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Consegna>()
            .HasIndex(c => c.ChiaveConsegna)
            .IsUnique();

        modelBuilder.Entity<Operatore>().HasData(
            new Operatore { OperatoreId = 1, Nome = "Mario", Cognome = "Rossi", Email = "mario.rossi@corriere.it", Password = BCrypt.Net.BCrypt.HashPassword("password") },
            new Operatore { OperatoreId = 2, Nome = "Laura", Cognome = "Bianchi", Email = "laura.bianchi@corriere.it", Password = BCrypt.Net.BCrypt.HashPassword("password") }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { ClienteId = 1, Nominativo = "Alessandro Verdi", Via = "Via Roma 10", Comune = "Milano", Provincia = "MI", Telefono = "023456789", Email = "alessandro.verdi@email.it" },
            new Cliente { ClienteId = 2, Nominativo = "Giovanni Neri", Via = "Corso Italia 25", Comune = "Roma", Provincia = "RM", Telefono = "069876543", Email = "giovanni.neri@email.it" },
            new Cliente { ClienteId = 3, Nominativo = "Maria Gialli", Via = "Piazza Duomo 5", Comune = "Firenze", Provincia = "FI", Telefono = "055123456", Email = "maria.gialli@email.it" }
        );

        modelBuilder.Entity<Consegna>().HasData(
            new Consegna { ConsegnaId = 1, ClienteId = 1, DataRitiro = new DateTime(2026, 5, 20), DataConsegna = new DateTime(2026, 5, 21), Stato = "Consegnata", ChiaveConsegna = "TRK-2026-000001" },
            new Consegna { ConsegnaId = 2, ClienteId = 2, DataRitiro = new DateTime(2026, 5, 22), DataConsegna = null, Stato = "In consegna", ChiaveConsegna = "TRK-2026-000002" },
            new Consegna { ConsegnaId = 3, ClienteId = 1, DataRitiro = new DateTime(2026, 5, 23), DataConsegna = null, Stato = "In deposito", ChiaveConsegna = "TRK-2026-000003" },
            new Consegna { ConsegnaId = 4, ClienteId = 3, DataRitiro = new DateTime(2026, 5, 24), DataConsegna = null, Stato = "Da ritirare", ChiaveConsegna = "TRK-2026-000004" },
            new Consegna { ConsegnaId = 5, ClienteId = 2, DataRitiro = new DateTime(2026, 5, 25), DataConsegna = null, Stato = "In giacenza", ChiaveConsegna = "TRK-2026-000005" },
            new Consegna { ConsegnaId = 6, ClienteId = 3, DataRitiro = new DateTime(2026, 5, 10), DataConsegna = new DateTime(2026, 5, 11), Stato = "Consegnata", ChiaveConsegna = "TRK-2026-000006" }
        );
    }
}
