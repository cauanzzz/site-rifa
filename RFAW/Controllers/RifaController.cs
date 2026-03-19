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
        public string CompradorEmail { get; set; }
    }

    public class PedidoAprovacao
    {
        public int RifaId { get; set; }
        public int Numero { get; set; }
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
            var rifasResumo = await _context.Rifas
                .Select(r => new
                {
                    Id = r.Id,
                    Titulo = r.Titulo,
                    Descricao = r.Descricao,
                    Premio = r.Premio,
                    Preço = r.Preço,
                    QuantidadeCotas = r.QuantidadeCotas,
                    Imagem = r.Imagem,
                    DataSorteio = r.DataSorteio,
                    CriadorEmail = r.CriadorEmail,
                    CriadorNome = _context.Usuarios
                        .Where(u => u.Email == r.CriadorEmail)
                        .Select(u => u.Nome)
                        .FirstOrDefault(),

                    Cotas = r.Cotas
                        .Where(c => c.Status != "Disponivel")
                        .Select(c => new
                        {
                            Numero = c.Numero,
                            Status = c.Status,
                            CompradorEmail = c.CompradorEmail,
                            NomePagador = c.Nome,
                            FormaPagamento = c.Tel,
                            DataReserva = c.DataReserva
                        }),
                    CotasVendidas = r.Cotas.Count(c => c.Status != "Disponivel")
                })
                .ToListAsync();

            return Ok(rifasResumo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRifaCompleta(int id)
        {
            var rifa = await _context.Rifas.Include(r => r.Cotas).FirstOrDefaultAsync(r => r.Id == id);
            if (rifa == null) return NotFound("Rifa não encontrada.");
            return Ok(rifa);
        }

        [HttpPost]
        public async Task<IActionResult> CriarRifa([FromBody] Rifa novaRifa)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == novaRifa.CriadorEmail);
            if (usuario == null) return BadRequest("Usuário não encontrado.");

            int custoMoedas = (int)Math.Ceiling(novaRifa.QuantidadeCotas / 10.0);

            if (!usuario.IsAdmin)
            {
                if (usuario.Moedas < custoMoedas)
                {
                    return BadRequest($"Saldo insuficiente. Você precisa de {custoMoedas} moedas, mas tem apenas {usuario.Moedas}.");
                }
                usuario.Moedas -= custoMoedas;
            }

            if (novaRifa.Cotas == null) novaRifa.Cotas = new List<Cota>();

            for (int i = 1; i <= novaRifa.QuantidadeCotas; i++)
            {
                novaRifa.Cotas.Add(new Cota { Numero = i, Status = "Disponivel" });
            }

            _context.Rifas.Add(novaRifa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Rifa criada com sucesso!",
                id = novaRifa.Id,
                moedasRestantes = usuario.Moedas
            });
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
                    cota.CompradorEmail = pedido.CompradorEmail;
                    cota.DataReserva = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Números reservados com sucesso!" });
        }

        [HttpPost("aprovar")]
        public async Task<IActionResult> AprovarPagamento([FromBody] PedidoAprovacao pedido)
        {
            var rifa = await _context.Rifas.Include(r => r.Cotas).FirstOrDefaultAsync(r => r.Id == pedido.RifaId);
            if (rifa == null) return NotFound("Rifa não encontrada.");

            var cota = rifa.Cotas.FirstOrDefault(c => c.Numero == pedido.Numero);
            if (cota != null && cota.Status == "Reservado")
            {
                cota.Status = "Vendido";
                await _context.SaveChangesAsync();
                return Ok(new { mensagem = "Pagamento aprovado com sucesso!" });
            }

            return BadRequest("Número não encontrado ou não está reservado.");
        }

        [HttpPost("rejeitar")]
        public async Task<IActionResult> RejeitarPagamento([FromBody] PedidoAprovacao pedido)
        {
            var rifa = await _context.Rifas.Include(r => r.Cotas).FirstOrDefaultAsync(r => r.Id == pedido.RifaId);
            if (rifa == null) return NotFound("Rifa não encontrada.");

            var cota = rifa.Cotas.FirstOrDefault(c => c.Numero == pedido.Numero);
            if (cota != null && cota.Status == "Reservado")
            {
                cota.Status = "Disponivel";
                cota.Nome = "";
                cota.Tel = "";
                cota.CompradorEmail = "";
                cota.DataReserva = null;
                await _context.SaveChangesAsync();
                return Ok(new { mensagem = "Reserva cancelada com sucesso!" });
            }

            return BadRequest("Número não encontrado ou não está reservado.");
        }

        [HttpGet("promover/{email}")]
        public async Task<IActionResult> PromoverAdmin(string email)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null) return NotFound("Usuário não encontrado.");

            usuario.IsAdmin = true;
            usuario.Moedas = 999999;

            await _context.SaveChangesAsync();
            return Ok($"Parabéns! O usuário {email} agora é o ADMINISTRADOR SUPREMO do sistema! 👑");
        }
        [HttpGet("buscar-por-criador")]
        public async Task<IActionResult> BuscarRifasPorCriador([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("Digite um nome para buscar.");
            }

            var rifas = await _context.Rifas
                .Where(r => _context.Usuarios.Any(u => u.Email == r.CriadorEmail && u.Nome.Contains(nome)))
                .Select(r => new
                {
                    Id = r.Id,
                    Titulo = r.Titulo,
                    Descricao = r.Descricao,
                    Premio = r.Premio,
                    Preço = r.Preço,
                    QuantidadeCotas = r.QuantidadeCotas,
                    Imagem = r.Imagem,
                    DataSorteio = r.DataSorteio,
                    CriadorEmail = r.CriadorEmail,
                    CriadorNome = _context.Usuarios.Where(u => u.Email == r.CriadorEmail).Select(u => u.Nome).FirstOrDefault(),
                    Cotas = r.Cotas
                        .Where(c => c.Status != "Disponivel")
                        .Select(c => new
                        {
                            Numero = c.Numero,
                            Status = c.Status,
                            CompradorEmail = c.CompradorEmail,
                            NomePagador = c.Nome,
                            FormaPagamento = c.Tel,
                            DataReserva = c.DataReserva
                        }),
                    CotasVendidas = r.Cotas.Count(c => c.Status != "Disponivel")
                })
                .ToListAsync();

            if (!rifas.Any())
            {
                return NotFound("Nenhuma rifa encontrada para este criador.");
            }

            return Ok(rifas);
        }
    }
}