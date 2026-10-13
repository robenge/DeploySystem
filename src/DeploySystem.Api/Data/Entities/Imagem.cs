namespace DeploySystem.Api.Data.Entities;

public class Imagem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Versao { get; set; } = string.Empty;
    public string CaminhoWim { get; set; } = string.Empty;
    public string? Hash { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Perfil> Perfis { get; set; } = new List<Perfil>();
}
