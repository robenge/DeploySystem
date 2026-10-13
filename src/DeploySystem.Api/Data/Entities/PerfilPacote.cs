namespace DeploySystem.Api.Data.Entities;

public class PerfilPacote
{
    public int PerfilId { get; set; }
    public Perfil Perfil { get; set; } = null!;

    public int PacoteId { get; set; }
    public Pacote Pacote { get; set; } = null!;

    public int Ordem { get; set; }
}
