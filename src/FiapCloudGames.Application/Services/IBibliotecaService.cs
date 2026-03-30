using FiapCloudGames.Application.DTOs.Response;

namespace FiapCloudGames.Application.Services;

public interface IBibliotecaService
{
    Task AdquirirJogoAsync(string usuarioId, string jogoId, CancellationToken ct = default);
    Task<IEnumerable<JogoResponseDto>> ListarBibliotecaAsync(string usuarioId, CancellationToken ct = default);
}
