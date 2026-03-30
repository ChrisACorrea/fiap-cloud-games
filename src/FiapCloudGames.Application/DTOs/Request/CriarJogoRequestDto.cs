using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Application.DTOs.Request;

public sealed record CriarJogoRequestDto(
    string Titulo,
    string Descricao,
    GeneroJogo Genero,
    decimal Preco,
    DateTime DataLancamento);
