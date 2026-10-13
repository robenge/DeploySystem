namespace DeploySystem.Api.Data.Entities;

public class Pacote
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;   // Msi, Exe, PowerShell, Winget, Chocolatey
    public string Caminho { get; set; } = string.Empty;
    public string? Argumentos { get; set; }
    public string? Hash { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<PerfilPacote> PerfilPacotes { get; set; } = new List<PerfilPacote>();
}
