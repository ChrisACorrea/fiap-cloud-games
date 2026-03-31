using System.Security.Claims;
using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.API.Controllers;

/// <summary>
/// Controller responsável pela biblioteca de jogos do usuário.
/// </summary>
[ApiController]
[Route("api/v1/biblioteca")]
[Produces("application/json")]
[Authorize(Policy = "UsuarioAutenticado")]
public sealed class BibliotecaController(IBibliotecaService bibliotecaService) : ControllerBase
{
    /// <summary>
    /// Listar jogos da biblioteca do usuário autenticado.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de jogos adquiridos.</returns>
    /// <response code="200">Biblioteca retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JogoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var usuarioId = ObterUsuarioId();

        var resultado = await bibliotecaService.ListarBibliotecaAsync(usuarioId, ct);

        return Ok(resultado);
    }

    /// <summary>
    /// Adquirir jogo para a biblioteca do usuário autenticado.
    /// </summary>
    /// <param name="dto">Dados do jogo a adquirir.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="201">Jogo adquirido com sucesso.</response>
    /// <response code="404">Jogo não encontrado.</response>
    /// <response code="409">Jogo já adquirido.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Adquirir([FromBody] AdquirirJogoRequestDto dto, CancellationToken ct)
    {
        var usuarioId = ObterUsuarioId();

        await bibliotecaService.AdquirirJogoAsync(usuarioId, dto.JogoId, ct);

        return StatusCode(StatusCodes.Status201Created);
    }

    private string ObterUsuarioId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new AcessoNegadoException();
}
