# Changelog

Todas as mudancas relevantes do projeto estao documentadas neste arquivo.

O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e o projeto adere ao [Versionamento Semantico](https://semver.org/lang/pt-BR/).

## [1.3.0] - 2026-04-04

### Alterado
- Permitir acesso sem autenticação ao catálogo de jogos:
  - Listagem paginada (`GET /api/v1/jogos`)
  - Busca de jogo por ID (`GET /api/v1/jogos/{id}`)
  - Atualizar testes de integração do `JogosController` para refletir os endpoints públicos

### Corrigido
- Corrigir configuração do Swagger para enviar token JWT nas requisições autenticadas:
  - Alterar `SecuritySchemeType` de `ApiKey` para `Http` com scheme `bearer`
  - Passar `document` ao `OpenApiSecuritySchemeReference` para serialização correta do requisito de segurança

## [1.2.0] - 2026-03-31

### Alterado
- Migrar projeto de .NET 8 para .NET 10 LTS (suporte ate Nov 2028)
- Atualizar todos os pacotes NuGet para versoes compativeis com .NET 10
- Corrigir breaking changes do OpenAPI 2.0 (Swashbuckle 10.x)
- Adotar Named Query Filter (EF Core 10) para soft-delete de jogos

## [1.1.0] - 2026-03-31

### Adicionado
- 13 endpoints REST: Auth, Usuarios (CRUD), Jogos (CRUD), Biblioteca
- Validacao com FluentValidation na fronteira da API
- Extension `ValidationExtensions.ValidarAsync` para integrar FluentValidation com excepcoes de dominio
- Autorizacao por policies JWT (Publico, Autenticado, Admin, Proprio ou Admin)
- Extracao de userId via claims JWT no BibliotecaController
- 177 testes automatizados (151 unitarios + 26 integracao)
- Testes de integracao com Testcontainers MongoDB + WebApplicationFactory
- Cobertura de entidades, services, validators e endpoints
- README.md completo com instrucoes de uso e documentacao da API
- Documentacao DDD com Event Storming (docs/DDD.md)
- Linguagem Ubiqua, Contextos Delimitados, Mapa de Agregados, Invariantes

### Alterado
- Migrar persistencia de MongoDB.Driver direto para Entity Framework Core com MongoDB Provider
- Gerar ObjectIds via ValueGenerator com Sentinel para compatibilidade com EF Core
- Configurar AutoTransactionBehavior.Never para compatibilidade com MongoDB standalone

### Removido
- BsonMappings.cs (serializers customizados substituidos por EF Core Fluent API)
- MongoDbIndexes.cs (indices manuais substituidos por HasIndex no Fluent API)

## [1.0.0] - 2026-03-30

### Adicionado
- Estrutura da solucao .NET 8 com Clean Architecture (Domain, Application, Infrastructure, API)
- Camada de Dominio: entidades (Usuario, Jogo, BibliotecaJogo), Value Objects (Email, Preco, Senha), Enums, Excecoes
- Camada de Aplicacao: services (Auth, Usuario, Jogo, Biblioteca), DTOs, Validators (FluentValidation)
- Camada de Infraestrutura: repositorios MongoDB, autenticacao JWT (HS256), BCrypt (work factor 12), seed de dados
- Middleware global de tratamento de erros com ProblemDetails (RFC 7807)
- Logging estruturado com Serilog (JSON + enrichers)
- Swagger/OpenAPI com documentacao em pt-BR e autenticacao JWT
- MongoDB com Docker Compose
- Seed automatico: administrador (admin@fcg.com) e 5 jogos de exemplo
- 98 testes unitarios (entidades, VOs, services, validators)
