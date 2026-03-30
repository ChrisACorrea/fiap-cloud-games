using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Repositories;
using MongoDB.Driver;

namespace FiapCloudGames.Infrastructure.Repositories;

public sealed class BibliotecaRepository(IMongoDatabase database) : IBibliotecaRepository
{
    private readonly IMongoCollection<BibliotecaJogo> _collection = database.GetCollection<BibliotecaJogo>("biblioteca_jogos");

    public async Task<IEnumerable<BibliotecaJogo>> ObterPorUsuarioAsync(string usuarioId, CancellationToken ct = default) =>
        await _collection.Find(b => b.UsuarioId == usuarioId).ToListAsync(ct);

    public async Task AdicionarJogoAsync(BibliotecaJogo bibliotecaJogo, CancellationToken ct = default) =>
        await _collection.InsertOneAsync(bibliotecaJogo, cancellationToken: ct);

    public async Task<bool> UsuarioPossuiJogoAsync(string usuarioId, string jogoId, CancellationToken ct = default) =>
        await _collection.Find(b => b.UsuarioId == usuarioId && b.JogoId == jogoId).AnyAsync(ct);
}
