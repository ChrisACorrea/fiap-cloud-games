using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Repositories;
using MongoDB.Driver;

namespace FiapCloudGames.Infrastructure.Repositories;

public sealed class UsuarioRepository(IMongoDatabase database) : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _collection = database.GetCollection<Usuario>("usuarios");

    public async Task<Usuario?> ObterPorIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        await _collection.Find(u => u.Email == new Domain.ValueObjects.Email(email)).FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Usuario>> ObterTodosAsync(int pagina, int tamanhoPagina, CancellationToken ct = default) =>
        await _collection.Find(FilterDefinition<Usuario>.Empty)
            .Skip((pagina - 1) * tamanhoPagina)
            .Limit(tamanhoPagina)
            .ToListAsync(ct);

    public async Task CriarAsync(Usuario usuario, CancellationToken ct = default) =>
        await _collection.InsertOneAsync(usuario, cancellationToken: ct);

    public async Task AtualizarAsync(Usuario usuario, CancellationToken ct = default) =>
        await _collection.ReplaceOneAsync(u => u.Id == usuario.Id, usuario, cancellationToken: ct);

    public async Task RemoverAsync(string id, CancellationToken ct = default) =>
        await _collection.DeleteOneAsync(u => u.Id == id, ct);

    public async Task<bool> EmailExisteAsync(string email, CancellationToken ct = default) =>
        await _collection.Find(u => u.Email == new Domain.ValueObjects.Email(email)).AnyAsync(ct);
}
