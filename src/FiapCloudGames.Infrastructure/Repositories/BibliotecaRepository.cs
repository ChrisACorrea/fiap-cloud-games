using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Repositories;
using FiapCloudGames.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure.Repositories;

public sealed class BibliotecaRepository(AppDbContext context) : IBibliotecaRepository
{
    public async Task<IEnumerable<BibliotecaJogo>> ObterPorUsuarioAsync(string usuarioId, CancellationToken ct = default) =>
        await context.BibliotecaJogos
            .Where(b => b.UsuarioId == usuarioId)
            .ToListAsync(ct);

    public async Task AdicionarJogoAsync(BibliotecaJogo bibliotecaJogo, CancellationToken ct = default)
    {
        context.BibliotecaJogos.Add(bibliotecaJogo);
        await context.SaveChangesAsync(ct);
    }

    public async Task<bool> UsuarioPossuiJogoAsync(string usuarioId, string jogoId, CancellationToken ct = default) =>
        await context.BibliotecaJogos
            .AnyAsync(b => b.UsuarioId == usuarioId && b.JogoId == jogoId, ct);
}
