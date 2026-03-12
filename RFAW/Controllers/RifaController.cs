using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFAW.Data;
using RFAW.Models;

namespace RFAW.Controllers
{
    public class PedidoCompra
    {
        public int RifaId { get; set; }
        public List<int> Numeros { get; set; }
        public string NomePagador { get; set; }
        public string FormaPagamento { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class RifaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RifaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRifas()
        {
            var rifas = await _context.Rifas.Include(r => r.Cotas).ToListAsync();
            return Ok(rifas);
        }

        [HttpPost]
        public async Task<IActionResult> CriarRifa([FromBody] Rifa novaRifa)
        {
            if (novaRifa.Cotas == null)
            {
                novaRifa.Cotas = new List<Cota>();
            }

            for (int i = 1; i <= novaRifa.QuantidadeCotas; i++)
            {
                novaRifa.Cotas.Add(new Cota
                {
                    Numero = i,
                    Status = "Disponivel"
                });
            }

            _context.Rifas.Add(novaRifa);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Rifa criada com sucesso!", id = novaRifa.Id });
        }

        [HttpPost("comprar")]
        public async Task<IActionResult> ComprarNumeros([FromBody] PedidoCompra pedido)
        {
            var rifa = await _context.Rifas.Include(r => r.Cotas).FirstOrDefaultAsync(r => r.Id == pedido.RifaId);

            if (rifa == null) return NotFound("Rifa não encontrada.");
            foreach (var numero in pedido.Numeros)
            {
                var cota = rifa.Cotas.FirstOrDefault(c => c.Numero == numero);
                if (cota != null && cota.Status == "Disponivel")
                {
                    cota.Status = "Reservado"; 
                    cota.Nome = pedido.NomePagador;
                    cota.Tel = pedido.FormaPagamento;
                }
            }


            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Números reservados com sucesso!" });
        }
    }
}