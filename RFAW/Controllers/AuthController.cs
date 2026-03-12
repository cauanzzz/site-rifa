using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFAW.Data;
using RFAW.Models;

namespace RFAW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario loginInfo)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginInfo.Email && u.Senha == loginInfo.Senha);

            if (usuario == null)
            {
                return Unauthorized(new { mensagem = "E-mail ou senha incorretos!" });
            }

            return Ok(new
            {
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email,
                isAdmin = usuario.IsAdmin
            });
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Usuario novoUsuario)
        {
            var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == novoUsuario.Email);

            if (emailJaExiste)
            {
                return BadRequest(new { mensagem = "Esse e-mail já está cadastrado!" });
            }
            novoUsuario.IsAdmin = false;

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Cadastro realizado com sucesso! Agora você já pode fazer login." });
        }
    }
}