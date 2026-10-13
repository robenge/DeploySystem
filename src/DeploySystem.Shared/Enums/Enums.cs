namespace DeploySystem.Shared.Enums;

public enum StatusDeploy
{
    Pendente,
    AplicandoImagem,
    InstalandoPacotes,
    Concluido,
    Falha,
    Cancelado
}

public enum TipoPacote
{
    Msi,
    Exe,
    PowerShell,
    Winget,
    Chocolatey
}

public enum PerfilUsuario
{
    Admin,
    User
}
