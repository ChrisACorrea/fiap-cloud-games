namespace FiapCloudGames.Application.DTOs.Request;

public sealed record LoginRequestDto(
    string Email,
    string Senha);
