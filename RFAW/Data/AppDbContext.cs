using Microsoft.EntityFrameworkCore;
using RFAW.Models;

namespace RFAW.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Rifa> Rifas { get; set; }
        public DbSet<Cota> Cotas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<MensagemSuporte> MensagensSuporte { get; set; }
    }
}