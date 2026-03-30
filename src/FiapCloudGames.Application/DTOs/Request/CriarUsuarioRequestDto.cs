namespace FiapCloudGames.Application.DTOs.Request;

public sealed record CriarUsuarioRequestDto(
    string Nome,
    string Email,
    string Senha);
