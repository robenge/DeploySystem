using DeploySystem.Api.Data;
using DeploySystem.Api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeploySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagensController : ControllerBase
{
    private readonly DeployDbContext _db;

    public ImagensController(DeployDbContext db) => _db = db;

    public record ImagemInput(string Nome, string Versao, string CaminhoWim, string? Hash);

    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _db.Imagens.OrderByDescending(i => i.CriadoEm).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var img = await _db.Imagens.FindAsync(id);
        return img is null ? NotFound() : Ok(img);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] ImagemInput input)
    {
        var imagem = new Imagem
        {
            Nome = input.Nome,
            Versao = input.Versao,
            CaminhoWim = input.CaminhoWim,
            Hash = input.Hash
        };

        _db.Imagens.Add(imagem);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = imagem.Id }, imagem);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Editar(int id, [FromBody] ImagemInput input)
    {
        var imagem = await _db.Imagens.FindAsync(id);
        if (imagem is null) return NotFound();

        imagem.Nome = input.Nome;
        imagem.Versao = input.Versao;
        imagem.CaminhoWim = input.CaminhoWim;
        imagem.Hash = input.Hash;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var imagem = await _db.Imagens.FindAsync(id);
        if (imagem is null) return NotFound();

        _db.Imagens.Remove(imagem);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
