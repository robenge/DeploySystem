namespace DeploySystem.Api.Data.Entities;

public class Perfil
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;

    public int ImagemId { get; set; }
    public Imagem Imagem { get; set; } = null!;

    public ICollection<PerfilPacote> PerfilPacotes { get; set; } = new List<PerfilPacote>();
    public ICollection<Deploy> Deploys { get; set; } = new List<Deploy>();
}
