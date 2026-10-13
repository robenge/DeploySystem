namespace DeploySystem.Shared.Dtos;

public record PerfilDto(
    int Id,
    string Codigo,
    string Nome,
    string? Descricao,
    int ImagemId,
    bool Ativo,
    List<PacoteDto> Pacotes);

public record PacoteDto(
    int Id,
    string Nome,
    string Tipo,
    string Caminho,
    string? Argumentos,
    int Ordem);

public record DeployDto(
    int Id,
    string CodigoPerfil,
    string Hostname,
    string MacAddress,
    string Status,
    int Progresso,
    DateTime IniciadoEm,
    DateTime? FinalizadoEm);

public record IniciarDeployRequest(
    string Codigo,
    string Hostname,
    string MacAddress);

public record IniciarDeployResponse(
    int DeployId,
    string ImagemWim,
    List<PacoteDto> Pacotes,
    string Hostname);
