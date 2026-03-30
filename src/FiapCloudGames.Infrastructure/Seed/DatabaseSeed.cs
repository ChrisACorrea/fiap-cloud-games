using FiapCloudGames.Application.Interfaces;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace FiapCloudGames.Infrastructure.Seed;

public static class DatabaseSeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        using var scope = serviceProvider.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IMongoDatabase>>();

        await SeedUsuarioAdminAsync(database, passwordHasher, logger, ct);
        await SeedJogosAsync(database, logger, ct);
    }

    private static async Task SeedUsuarioAdminAsync(
        IMongoDatabase database,
        IPasswordHasher passwordHasher,
        ILogger logger,
        CancellationToken ct)
    {
        var collection = database.GetCollection<Usuario>("usuarios");
        var adminExiste = await collection.Find(u => u.Email == new Email("admin@fcg.com")).AnyAsync(ct);

        if (adminExiste)
        {
            logger.LogInformation("Seed: Usuário administrador já existe. Pulando...");
            return;
        }

        var senhaHash = passwordHasher.Hash("Admin@123456");
        var admin = new Usuario("Administrador FCG", new Email("admin@fcg.com"), senhaHash, TipoUsuario.Administrador);

        await collection.InsertOneAsync(admin, cancellationToken: ct);
        logger.LogInformation("Seed: Usuário administrador criado com sucesso");
    }

    private static async Task SeedJogosAsync(
        IMongoDatabase database,
        ILogger logger,
        CancellationToken ct)
    {
        var collection = database.GetCollection<Jogo>("jogos");
        var count = await collection.CountDocumentsAsync(FilterDefinition<Jogo>.Empty, cancellationToken: ct);

        if (count > 0)
        {
            logger.LogInformation("Seed: Jogos já existem ({Count} encontrados). Pulando...", count);
            return;
        }

        var jogos = new[]
        {
            new Jogo("CodeQuest: A Jornada do Desenvolvedor", "Um RPG educacional onde você aprende programação enquanto evolui seu personagem.", GeneroJogo.RPG, new Preco(49.90m), new DateTime(2024, 3, 15)),
            new Jogo("Bug Hunter: Caçador de Defeitos", "Resolva puzzles baseados em bugs reais de software.", GeneroJogo.Puzzle, new Preco(29.90m), new DateTime(2024, 5, 20)),
            new Jogo("Cloud Arena: Batalha dos Servidores", "Gerencie infraestrutura cloud em tempo real e vença seus oponentes.", GeneroJogo.Estrategia, new Preco(59.90m), new DateTime(2024, 7, 10)),
            new Jogo("API Rush: Corrida das Requisições", "Uma corrida frenética onde suas requisições competem pela melhor performance.", GeneroJogo.Corrida, new Preco(39.90m), new DateTime(2024, 9, 1)),
            new Jogo("Cyber Shield: Defesa Digital", "Defenda sua rede contra ataques cibernéticos em tempo real.", GeneroJogo.Acao, new Preco(44.90m), new DateTime(2024, 11, 25))
        };

        await collection.InsertManyAsync(jogos, cancellationToken: ct);
        logger.LogInformation("Seed: {Count} jogos de exemplo criados com sucesso", jogos.Length);
    }
}
