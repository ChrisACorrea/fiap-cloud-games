using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Application.DTOs.Response;

public sealed record LoginResponseDto(
    string Token,
    DateTime Expiracao,
    TipoUsuario Tipo);
