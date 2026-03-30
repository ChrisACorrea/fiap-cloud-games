using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.Repositories;
using MongoDB.Driver;

namespace FiapCloudGames.Infrastructure.Repositories;

public sealed class JogoRepository(IMongoDatabase database) : IJogoRepository
{
    private readonly IMongoCollection<Jogo> _collection = database.GetCollection<Jogo>("jogos");

    public async Task<Jogo?> ObterPorIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(j => j.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Jogo>> ObterTodosAsync(int pagina, int tamanhoPagina, CancellationToken ct = default) =>
        await _collection.Find(j => j.Ativo)
            .Skip((pagina - 1) * tamanhoPagina)
            .Limit(tamanhoPagina)
            .ToListAsync(ct);

    public async Task<IEnumerable<Jogo>> BuscarPorGeneroAsync(GeneroJogo genero, int pagina, int tamanhoPagina, CancellationToken ct = default) =>
        await _collection.Find(j => j.Ativo && j.Genero == genero)
            .Skip((pagina - 1) * tamanhoPagina)
            .Limit(tamanhoPagina)
            .ToListAsync(ct);

    public async Task CriarAsync(Jogo jogo, CancellationToken ct = default) =>
        await _collection.InsertOneAsync(jogo, cancellationToken: ct);

    public async Task AtualizarAsync(Jogo jogo, CancellationToken ct = default) =>
        await _collection.ReplaceOneAsync(j => j.Id == jogo.Id, jogo, cancellationToken: ct);

    public async Task RemoverAsync(string id, CancellationToken ct = default) =>
        await _collection.DeleteOneAsync(j => j.Id == id, ct);

    public async Task<bool> TituloExisteAsync(string titulo, CancellationToken ct = default) =>
        await _collection.Find(j => j.Titulo == titulo).AnyAsync(ct);
}
