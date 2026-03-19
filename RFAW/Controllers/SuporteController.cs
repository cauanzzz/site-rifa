using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFAW.Data;
using RFAW.Models;

namespace RFAW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuporteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuporteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ReceberMensagemSuporte([FromBody] MensagemSuporte novaMensagem)
        {
            if (string.IsNullOrEmpty(novaMensagem.Email) || string.IsNullOrEmpty(novaMensagem.Mensagem))
            {
                return BadRequest("E-mail e mensagem são obrigatórios!");
            }
            novaMensagem.DataEnvio = DateTime.Now;

            _context.MensagensSuporte.Add(novaMensagem);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Mensagem enviada com sucesso!" });
        }

        [HttpGet]
        public async Task<IActionResult> VerMensagensSuporte()
        {
            var mensagens = await _context.MensagensSuporte
                                          .OrderByDescending(m => m.DataEnvio)
                                          .ToListAsync();
            return Ok(mensagens);
        }
    }
}