using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Application.Services;

public interface IJogoService
{
    Task<JogoResponseDto> CriarAsync(CriarJogoRequestDto dto, CancellationToken ct = default);
    Task<JogoResponseDto> ObterPorIdAsync(string id, CancellationToken ct = default);
    Task<PaginacaoResponseDto<JogoResponseDto>> ListarAsync(int pagina, int tamanhoPagina, GeneroJogo? genero, CancellationToken ct = default);
    Task<JogoResponseDto> AtualizarAsync(string id, AtualizarJogoRequestDto dto, CancellationToken ct = default);
    Task RemoverAsync(string id, CancellationToken ct = default);
}
