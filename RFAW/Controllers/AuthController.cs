using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFAW.Data;
using RFAW.Models;
using BCrypt.Net;

namespace RFAW.Controllers
{
    public class DadosLogin
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario novoUsuario)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Email == novoUsuario.Email);
            if (existe) return BadRequest("E-mail já cadastrado!");

            novoUsuario.Moedas = 50;
            novoUsuario.Senha = BCrypt.Net.BCrypt.HashPassword(novoUsuario.Senha);

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();
            return Ok(novoUsuario);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DadosLogin loginDados)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDados.Email);

            if (usuario == null)
            {
                return Unauthorized("E-mail ou senha incorretos.");
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(loginDados.Senha, usuario.Senha))
                {
                    return Unauthorized("E-mail ou senha incorretos.");
                }
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return Unauthorized("E-mail ou senha incorretos.");
            }

            return Ok(usuario);
        }
    }
}