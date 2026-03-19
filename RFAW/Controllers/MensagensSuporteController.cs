using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFAW.Data; 
using RFAW.Models; 
using System;
using System.Threading.Tasks;

namespace RFAW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensagensSuporteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MensagensSuporteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMensagens()
        {
            var mensagens = await _context.MensagensSuporte.ToListAsync();
            return Ok(mensagens);
        }


        [HttpPost]
        public async Task<IActionResult> PostMensagem(MensagemSuporte mensagem)
        {
            mensagem.DataEnvio = DateTime.Now;
            mensagem.Lida = false;

            _context.MensagensSuporte.Add(mensagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMensagens), new { id = mensagem.Id }, mensagem);
        }
    }
}