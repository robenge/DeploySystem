using DeploySystem.Api.Data;
using DeploySystem.Api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeploySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerfisController : ControllerBase
{
    private readonly DeployDbContext _db;

    public PerfisController(DeployDbContext db) => _db = db;

    public record PerfilInput(string Codigo, string Nome, string? Descricao, int ImagemId, bool Ativo);

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var perfis = await _db.Perfis
            .Include(p => p.Imagem)
            .Include(p => p.PerfilPacotes).ThenInclude(pp => pp.Pacote)
            .Select(p => new
            {
                p.Id, p.Codigo, p.Nome, p.Descricao, p.Ativo,
                Imagem = new { p.Imagem.Id, p.Imagem.Nome, p.Imagem.Versao },
                Pacotes = p.PerfilPacotes
                    .OrderBy(pp => pp.Ordem)
                    .Select(pp => new { pp.Pacote.Id, pp.Pacote.Nome, pp.Ordem })
            })
            .ToListAsync();

        return Ok(perfis);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var perfil = await _db.Perfis
            .Include(p => p.Imagem)
            .Include(p => p.PerfilPacotes).ThenInclude(pp => pp.Pacote)
            .FirstOrDefaultAsync(p => p.Id == id);

        return perfil is null ? NotFound() : Ok(perfil);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] PerfilInput input)
    {
        if (await _db.Perfis.AnyAsync(p => p.Codigo == input.Codigo))
            return BadRequest(new { mensagem = "Já existe um perfil com esse código." });

        var perfil = new Perfil
        {
            Codigo = input.Codigo,
            Nome = input.Nome,
            Descricao = input.Descricao,
            ImagemId = input.ImagemId,
            Ativo = input.Ativo
        };

        _db.Perfis.Add(perfil);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = perfil.Id }, perfil);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Editar(int id, [FromBody] PerfilInput input)
    {
        var perfil = await _db.Perfis.FindAsync(id);
        if (perfil is null) return NotFound();

        perfil.Codigo = input.Codigo;
        perfil.Nome = input.Nome;
        perfil.Descricao = input.Descricao;
        perfil.ImagemId = input.ImagemId;
        perfil.Ativo = input.Ativo;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var perfil = await _db.Perfis.FindAsync(id);
        if (perfil is null) return NotFound();

        _db.Perfis.Remove(perfil);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Vincular pacotes ao perfil com ordem
    public record VincularPacotesInput(List<int> PacoteIds);

    [HttpPost("{id}/pacotes")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> VincularPacotes(int id, [FromBody] VincularPacotesInput input)
    {
        var perfil = await _db.Perfis
            .Include(p => p.PerfilPacotes)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (perfil is null) return NotFound();

        // Remove vínculos antigos
        _db.PerfilPacotes.RemoveRange(perfil.PerfilPacotes);

        // Adiciona novos com ordem sequencial
        int ordem = 1;
        foreach (var pacoteId in input.PacoteIds)
        {
            _db.PerfilPacotes.Add(new PerfilPacote
            {
                PerfilId = id,
                PacoteId = pacoteId,
                Ordem = ordem++
            });
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensagem = "Pacotes vinculados.", total = input.PacoteIds.Count });
    }
}
