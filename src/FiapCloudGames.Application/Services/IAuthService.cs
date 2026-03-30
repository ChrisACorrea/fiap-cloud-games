using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;

namespace FiapCloudGames.Application.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
}
