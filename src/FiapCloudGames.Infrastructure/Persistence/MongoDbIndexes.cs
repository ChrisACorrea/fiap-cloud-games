using FiapCloudGames.Domain.Entities;
using MongoDB.Driver;

namespace FiapCloudGames.Infrastructure.Persistence;

public static class MongoDbIndexes
{
    public static async Task CreateAsync(IMongoDatabase database, CancellationToken ct = default)
    {
        await CreateUsuarioIndexesAsync(database, ct);
        await CreateJogoIndexesAsync(database, ct);
        await CreateBibliotecaIndexesAsync(database, ct);
    }

    private static async Task CreateUsuarioIndexesAsync(IMongoDatabase database, CancellationToken ct)
    {
        var collection = database.GetCollection<Usuario>("usuarios");

        var emailIndex = new CreateIndexModel<Usuario>(
            Builders<Usuario>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true, Name = "idx_email_unique" });

        await collection.Indexes.CreateOneAsync(emailIndex, cancellationToken: ct);
    }

    private static async Task CreateJogoIndexesAsync(IMongoDatabase database, CancellationToken ct)
    {
        var collection = database.GetCollection<Jogo>("jogos");

        var tituloIndex = new CreateIndexModel<Jogo>(
            Builders<Jogo>.IndexKeys.Ascending(j => j.Titulo),
            new CreateIndexOptions { Unique = true, Name = "idx_titulo_unique" });

        var generoIndex = new CreateIndexModel<Jogo>(
            Builders<Jogo>.IndexKeys.Ascending(j => j.Genero),
            new CreateIndexOptions { Name = "idx_genero" });

        await collection.Indexes.CreateManyAsync([tituloIndex, generoIndex], ct);
    }

    private static async Task CreateBibliotecaIndexesAsync(IMongoDatabase database, CancellationToken ct)
    {
        var collection = database.GetCollection<BibliotecaJogo>("biblioteca_jogos");

        var usuarioIndex = new CreateIndexModel<BibliotecaJogo>(
            Builders<BibliotecaJogo>.IndexKeys.Ascending(b => b.UsuarioId),
            new CreateIndexOptions { Name = "idx_usuario_id" });

        var compoundIndex = new CreateIndexModel<BibliotecaJogo>(
            Builders<BibliotecaJogo>.IndexKeys
                .Ascending(b => b.UsuarioId)
                .Ascending(b => b.JogoId),
            new CreateIndexOptions { Unique = true, Name = "idx_usuario_jogo_unique" });

        await collection.Indexes.CreateManyAsync([usuarioIndex, compoundIndex], ct);
    }
}
