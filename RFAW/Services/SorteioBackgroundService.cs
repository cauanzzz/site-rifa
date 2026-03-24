using Microsoft.EntityFrameworkCore;
using RFAW.Data;

namespace RFAW.Services
{
    public class SorteioBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SorteioBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var rifasParaSortear = await context.Rifas
                            .Include(r => r.Cotas)
                            .Where(r => r.Status != "Encerrada" && r.DataSorteio != null && r.DataSorteio <= DateTime.Now)
                            .ToListAsync();

                        foreach (var rifa in rifasParaSortear)
                        {
                            var cotasValidas = rifa.Cotas.Where(c => c.Status == "Vendido" || c.Status == "Reservado").ToList();

                            if (cotasValidas.Count > 0)
                            {
                                var random = new Random();
                                int indiceSorteado = random.Next(cotasValidas.Count);
                                var cotaGanhadora = cotasValidas[indiceSorteado];

                                rifa.NumeroSorteado = cotaGanhadora.Numero;
                                rifa.Status = "Encerrada";
                            }
                            else
                            {
                                rifa.Status = "Encerrada";
                                rifa.NumeroSorteado = 0;
                            }
                        }

                        if (rifasParaSortear.Count > 0)
                        {
                            await context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no robô de sorteio: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}