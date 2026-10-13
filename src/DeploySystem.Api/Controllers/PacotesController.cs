using DeploySystem.Api.Data;
using DeploySystem.Api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeploySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PacotesController : ControllerBase
{
    private readonly DeployDbContext _db;
    private readonly IConfiguration _config;

    public PacotesController(DeployDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public record PacoteInput(string Nome, string Tipo, string Caminho, string? Argumentos);

    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _db.Pacotes.OrderBy(p => p.Nome).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var pacote = await _db.Pacotes.FindAsync(id);
        return pacote is null ? NotFound() : Ok(pacote);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] PacoteInput input)
    {
        var pacote = new Pacote
        {
            Nome = input.Nome,
            Tipo = input.Tipo,
            Caminho = input.Caminho,
            Argumentos = input.Argumentos
        };

        _db.Pacotes.Add(pacote);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = pacote.Id }, pacote);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Editar(int id, [FromBody] PacoteInput input)
    {
        var pacote = await _db.Pacotes.FindAsync(id);
        if (pacote is null) return NotFound();

        pacote.Nome = input.Nome;
        pacote.Tipo = input.Tipo;
        pacote.Caminho = input.Caminho;
        pacote.Argumentos = input.Argumentos;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var pacote = await _db.Pacotes.FindAsync(id);
        if (pacote is null) return NotFound();

        _db.Pacotes.Remove(pacote);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Upload do arquivo do pacote
    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(500_000_000)] // 500 MB
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { mensagem = "Arquivo inválido." });

        var pastaPacotes = _config["DeployPaths:Pacotes"] ?? "C:\\DeploySystem\\pacotes";
        Directory.CreateDirectory(pastaPacotes);

        var nomeSeguro = Path.GetFileName(file.FileName);
        var destino = Path.Combine(pastaPacotes, nomeSeguro);

        using (var stream = System.IO.File.Create(destino))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new
        {
            mensagem = "Arquivo enviado.",
            caminho = destino,
            tamanho = file.Length
        });
    }
}
