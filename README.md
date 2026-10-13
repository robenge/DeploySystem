# DeploySystem

Sistema de deploy automatizado de Windows 11 com múltiplos perfis.

## Estrutura

- `src/DeploySystem.Shared` — DTOs, enums, contratos
- `src/DeploySystem.Api` — API REST (ASP.NET Core)
- `src/DeploySystem.Web` — Interface Blazor (Admin + User)
- `src/DeploySystem.Agent` — Agente Windows Service
- `deploy/` — Imagens WIM, pacotes, scripts

## Como rodar

```powershell
dotnet build
dotnet run --project src/DeploySystem.Api
dotnet run --project src/DeploySystem.Web
