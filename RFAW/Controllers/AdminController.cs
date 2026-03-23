using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFAW.Data;
using RFAW.Models;

namespace RFAW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("solicitar-moedas")]
        public async Task<IActionResult> SolicitarMoedas([FromBody] PedidoMoeda pedido)
        {
            try
            {
                pedido.Status = "Pendente";
                pedido.DataSolicitacao = DateTime.Now;

                _context.PedidosMoeda.Add(pedido);
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Solicitação salva com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message, detalhe = ex.InnerException?.Message });
            }
        }

        [HttpGet("pedidos-moedas")]
        public async Task<IActionResult> GetPedidosPendentes()
        {
            var pedidos = await _context.PedidosMoeda
                .Where(p => p.Status == "Pendente")
                .Join(_context.Usuarios,
                      pedido => pedido.UsuarioId,
                      usuario => usuario.Id,
                      (pedido, usuario) => new {
                          pedido.Id,
                          UsuarioNome = usuario.Nome,
                          UsuarioEmail = usuario.Email,
                          pedido.Pacote,
                          pedido.QuantidadeMoedas,
                          pedido.ValorPago,
                          pedido.DataSolicitacao,
                          NomeTitularPix = pedido.NomeTitularPix
                      })
                .ToListAsync();

            return Ok(pedidos);
        }

        [HttpPost("aprovar-pedido/{id}")]
        public async Task<IActionResult> AprovarPedido(int id)
        {
            var pedido = await _context.PedidosMoeda.FindAsync(id);
            if (pedido == null || pedido.Status != "Pendente")
                return BadRequest("Pedido inválido ou já processado.");

            var usuario = await _context.Usuarios.FindAsync(pedido.UsuarioId);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            pedido.Status = "Aprovado";
            usuario.Moedas += pedido.QuantidadeMoedas;

            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Pedido aprovado com sucesso!" });
        }

        [HttpPost("recusar-pedido/{id}")]
        public async Task<IActionResult> RecusarPedido(int id)
        {
            var pedido = await _context.PedidosMoeda.FindAsync(id);
            if (pedido == null || pedido.Status != "Pendente")
                return BadRequest("Pedido inválido ou já processado.");

            pedido.Status = "Recusado";
            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Pedido recusado com sucesso!" });
        }
    }
}