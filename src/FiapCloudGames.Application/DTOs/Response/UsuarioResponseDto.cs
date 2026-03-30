using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Application.DTOs.Response;

public sealed record UsuarioResponseDto(
    string Id,
    string Nome,
    string Email,
    TipoUsuario Tipo,
    DateTime DataCriacao,
    bool Ativo);
