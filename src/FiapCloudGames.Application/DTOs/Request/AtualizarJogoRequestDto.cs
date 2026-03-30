using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Application.DTOs.Request;

public sealed record AtualizarJogoRequestDto(
    string Titulo,
    string Descricao,
    GeneroJogo Genero,
    decimal Preco);
