using System.Net;
using System.Net.Http.Json;
using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.IntegrationTests.Fixtures;
using FluentAssertions;

namespace FiapCloudGames.IntegrationTests.Controllers;

public sealed class BibliotecaControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<string> CriarJogoERetornarIdAsync()
    {
        var adminToken = await ObterTokenAdminAsync();
        AutenticarComo(adminToken);
        var dto = new CriarJogoRequestDto($"Jogo {Guid.NewGuid():N}", "Desc", GeneroJogo.Acao, 29.90m, DateTime.Now);
        var response = await Client.PostAsJsonAsync("/api/v1/jogos", dto);
        var jogo = await response.Content.ReadFromJsonAsync<JogoResponseDto>();
        return jogo!.Id;
    }

    [Fact]
    public async Task Adquirir_DeveRetornar201_QuandoDadosValidos()
    {
        var jogoId = await CriarJogoERetornarIdAsync();
        var userToken = await ObterTokenUsuarioAsync();
        AutenticarComo(userToken);

        var response = await Client.PostAsJsonAsync("/api/v1/biblioteca", new AdquirirJogoRequestDto(jogoId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Adquirir_DeveRetornar409_QuandoJogoDuplicado()
    {
        var jogoId = await CriarJogoERetornarIdAsync();
        var userToken = await ObterTokenUsuarioAsync();
        AutenticarComo(userToken);
        await Client.PostAsJsonAsync("/api/v1/biblioteca", new AdquirirJogoRequestDto(jogoId));

        var response = await Client.PostAsJsonAsync("/api/v1/biblioteca", new AdquirirJogoRequestDto(jogoId));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Listar_DeveRetornarJogosAdquiridos()
    {
        var jogoId = await CriarJogoERetornarIdAsync();
        var userToken = await ObterTokenUsuarioAsync();
        AutenticarComo(userToken);
        await Client.PostAsJsonAsync("/api/v1/biblioteca", new AdquirirJogoRequestDto(jogoId));

        var response = await Client.GetAsync("/api/v1/biblioteca");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var jogos = await response.Content.ReadFromJsonAsync<List<JogoResponseDto>>();
        jogos.Should().ContainSingle(j => j.Id == jogoId);
    }

    [Fact]
    public async Task Listar_DeveRetornarVazio_QuandoSemJogos()
    {
        var userToken = await ObterTokenUsuarioAsync();
        AutenticarComo(userToken);

        var response = await Client.GetAsync("/api/v1/biblioteca");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var jogos = await response.Content.ReadFromJsonAsync<List<JogoResponseDto>>();
        jogos.Should().BeEmpty();
    }

    [Fact]
    public async Task Adquirir_DeveRetornar401_QuandoSemAuth()
    {
        var response = await Client.PostAsJsonAsync("/api/v1/biblioteca", new AdquirirJogoRequestDto("id"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Listar_DeveRetornar401_QuandoSemAuth()
    {
        var response = await Client.GetAsync("/api/v1/biblioteca");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
