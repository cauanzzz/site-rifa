using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFAW.Data;
using RFAW.Models;

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

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();
            return Ok(novoUsuario);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DadosLogin loginDados)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDados.Email && u.Senha == loginDados.Senha);

            if (usuario == null) return Unauthorized("E-mail ou senha incorretos.");

            return Ok(usuario);
        }
    }
}