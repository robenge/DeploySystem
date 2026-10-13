using DeploySystem.Api.Data;
using DeploySystem.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeploySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DeployDbContext _db;
    private readonly IPasswordService _pwd;
    private readonly ITokenService _token;

    public AuthController(DeployDbContext db, IPasswordService pwd, ITokenService token)
    {
        _db = db;
        _pwd = pwd;
        _token = token;
    }

    public record LoginRequest(string Login, string Senha);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Login == req.Login && u.Ativo);
        if (user is null || !_pwd.Verificar(req.Senha, user.SenhaHash))
            return Unauthorized(new { mensagem = "Login ou senha inválidos" });

        var token = _token.GerarToken(user);
        return Ok(new
        {
            token,
            usuario = new { user.Id, user.Nome, user.Login, user.Perfil }
        });
    }

    [HttpPost("seed-admin")]
    public async Task<IActionResult> SeedAdmin()
    {
        if (await _db.Usuarios.AnyAsync())
            return BadRequest(new { mensagem = "Já existem usuários cadastrados." });

        var admin = new Data.Entities.Usuario
        {
            Nome = "Administrador",
            Login = "admin",
            SenhaHash = _pwd.Hash("admin123"),
            Perfil = "Admin",
            Ativo = true
        };

        _db.Usuarios.Add(admin);
        await _db.SaveChangesAsync();

        return Ok(new { mensagem = "Admin criado.", login = "admin", senha = "admin123" });
    }
}
