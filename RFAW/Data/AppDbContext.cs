using Microsoft.EntityFrameworkCore;
using RFAW.Models;
using RFAW.Models.NovaPasta;

namespace RFAW.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rifa> Rifas { get; set; }
    public DbSet<Cota> Cotas { get; set; }
}