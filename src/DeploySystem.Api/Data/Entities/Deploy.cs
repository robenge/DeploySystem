namespace DeploySystem.Api.Data.Entities;

public class Deploy
{
    public int Id { get; set; }
    public string CodigoPerfil { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string? MacAddress { get; set; }
    public string Status { get; set; } = "Pendente";
    public int Progresso { get; set; }
    public string? PacoteAtual { get; set; }
    public string? Log { get; set; }
    public string? MensagemErro { get; set; }
    public DateTime IniciadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? FinalizadoEm { get; set; }
}
