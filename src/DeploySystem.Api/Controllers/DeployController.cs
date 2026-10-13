using DeploySystem.Api.Data;
using DeploySystem.Api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeploySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeployController : ControllerBase
{
    private readonly DeployDbContext _db;

    public DeployController(DeployDbContext db) => _db = db;

    // ============================================================
    // ENDPOINT CRÍTICO — Chamado pelo WinPE com o código do perfil
    // NÃO requer autenticação (o WinPE não tem token)
    // Em produção, proteja com uma API Key ou mTLS
    // ============================================================
    public record IniciarRequest(string Codigo, string Hostname, string? MacAddress);

    [HttpPost("iniciar")]
    [AllowAnonymous]
    public async Task<IActionResult> Iniciar([FromBody] IniciarRequest req)
    {
        var perfil = await _db.Perfis
            .Include(p => p.Imagem)
            .Include(p => p.PerfilPacotes).ThenInclude(pp => pp.Pacote)
            .FirstOrDefaultAsync(p => p.Codigo == req.Codigo && p.Ativo);

        if (perfil is null)
            return NotFound(new { mensagem = $"Perfil '{req.Codigo}' não encontrado ou inativo." });

        // Registra o deploy
        var deploy = new Deploy
        {
            CodigoPerfil = perfil.Codigo,
            Hostname = req.Hostname,
            MacAddress = req.MacAddress,
            Status = "Pendente",
            Progresso = 0
        };

        _db.Deploys.Add(deploy);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            deployId = deploy.Id,
            hostname = req.Hostname,
            imagem = new
            {
                perfil.Imagem.Nome,
                perfil.Imagem.Versao,
                perfil.Imagem.CaminhoWim,
                perfil.Imagem.Hash
            },
            pacotes = perfil.PerfilPacotes
                .OrderBy(pp => pp.Ordem)
                .Select(pp => new
                {
                    pp.Pacote.Id,
                    pp.Pacote.Nome,
                    pp.Pacote.Tipo,
                    pp.Pacote.Caminho,
                    pp.Pacote.Argumentos,
                    pp.Ordem
                })
        });
    }

    // ============================================================
    // AGENTE reporta status durante a instalação
    // ============================================================
    public record StatusRequest(string Status, int Progresso, string? PacoteAtual, string? MensagemErro, string? Log);

    [HttpPost("{id}/status")]
    [AllowAnonymous]
    public async Task<IActionResult> ReportarStatus(int id, [FromBody] StatusRequest req)
    {
        var deploy = await _db.Deploys.FindAsync(id);
        if (deploy is null) return NotFound();

        deploy.Status = req.Status;
        deploy.Progresso = req.Progresso;
        deploy.PacoteAtual = req.PacoteAtual;
        deploy.MensagemErro = req.MensagemErro;

        if (req.Log is not null)
            deploy.Log = string.IsNullOrEmpty(deploy.Log) ? req.Log : deploy.Log + "\n" + req.Log;

        if (req.Status == "Concluido" || req.Status == "Falha")
            deploy.FinalizadoEm = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { mensagem = "Status atualizado." });
    }

    // ============================================================
    // LISTAGEM — usada pela interface web
    // ============================================================
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Listar([FromQuery] string? status, [FromQuery] int limite = 50)
    {
        var query = _db.Deploys.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(d => d.Status == status);

        var lista = await query
            .OrderByDescending(d => d.IniciadoEm)
            .Take(limite)
            .ToListAsync();

        return Ok(lista);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Obter(int id)
    {
        var deploy = await _db.Deploys.FindAsync(id);
        return deploy is null ? NotFound() : Ok(deploy);
    }

    [HttpGet("em-andamento")]
    [Authorize]
    public async Task<IActionResult> EmAndamento()
    {
        var lista = await _db.Deploys
            .Where(d => d.Status == "Pendente" || d.Status == "Instalando" || d.Status == "AplicandoImagem")
            .OrderByDescending(d => d.IniciadoEm)
            .ToListAsync();

        return Ok(lista);
    }
}
