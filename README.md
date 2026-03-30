# FIAP Cloud Games (FCG)

Plataforma de venda de jogos digitais e gerenciamento de servidores de jogos online, desenvolvida como Tech Challenge da pós-graduação em Arquitetura de Sistemas .NET da FIAP.

## Sobre o Projeto

O FIAP Cloud Games é uma API REST construída com .NET 8, seguindo os princípios de Clean Architecture e Domain-Driven Design (DDD). A Fase 1 (MVP) contempla:

- Cadastro e autenticação de usuários (JWT)
- Gerenciamento de jogos (CRUD)
- Biblioteca de jogos adquiridos por usuário
- Dois perfis de acesso: **Usuário** e **Administrador**

## Estrutura da Solução

```
FiapCloudGames.sln
src/
  FiapCloudGames.Domain/             Entidades, Value Objects, Interfaces de repositório
  FiapCloudGames.Application/        Use Cases, DTOs, Validators (FluentValidation)
  FiapCloudGames.Infrastructure/     Repositórios MongoDB, JWT, serviços externos
  FiapCloudGames.API/                Controllers, Middleware, Program.cs
tests/
  FiapCloudGames.UnitTests/          Testes unitários (xUnit + FluentAssertions + Moq)
  FiapCloudGames.IntegrationTests/   Testes de integração (Testcontainers + WebApplicationFactory)
```

### Dependências entre camadas

```
Domain          (zero dependências externas)
Application     -> Domain
Infrastructure  -> Domain, Application
API             -> Application, Infrastructure (composition root)
```

## Tecnologias

- **Runtime:** .NET 8 / C# 12
- **Banco de dados:** MongoDB
- **Autenticação:** JWT (JSON Web Tokens)
- **Validação:** FluentValidation
- **Testes:** xUnit, FluentAssertions, Moq, Testcontainers
- **Documentação API:** Swagger / OpenAPI
- **Logging:** Serilog
- **CI/CD:** GitHub Actions

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (8.0.x)
- [Docker](https://www.docker.com/) (para MongoDB via Docker Compose)
- [Git](https://git-scm.com/)

## Como Executar

### 1. Clonar o repositório

```bash
git clone https://github.com/FelipeMorandini/fiap-cloud-games.git
cd fiap-cloud-games
```

### 2. Subir o MongoDB local

```bash
docker compose up -d
```

O MongoDB estará disponível em `mongodb://localhost:27017`. O health check garante que o banco está pronto antes de aceitar conexões.

### 3. Restaurar dependências e compilar

```bash
dotnet restore
dotnet build
```

### 4. Executar a API

```bash
dotnet run --project src/FiapCloudGames.API
```

A API estará disponível em `https://localhost:5001` (ou `http://localhost:5000`).

O Swagger UI pode ser acessado em `/swagger` no ambiente de desenvolvimento.

## Como Testar

### Testes automatizados

```bash
dotnet test
```

### Testes unitários

```bash
dotnet test tests/FiapCloudGames.UnitTests
```

### Testes de integração

```bash
dotnet test tests/FiapCloudGames.IntegrationTests
```

> Os testes de integração utilizam Testcontainers e requerem Docker em execução.

## Licença

Este projeto está licenciado sob a licença MIT. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
