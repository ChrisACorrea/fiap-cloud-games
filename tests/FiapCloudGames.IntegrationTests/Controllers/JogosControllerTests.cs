using System.Net;
using System.Net.Http.Json;
using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.IntegrationTests.Fixtures;
using FluentAssertions;

namespace FiapCloudGames.IntegrationTests.Controllers;

public sealed class JogosControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static string UniqueTitle(string prefix = "Jogo") => $"{prefix} {Guid.NewGuid():N}";

    private async Task<JogoResponseDto> CriarJogoAsync(string? titulo = null, GeneroJogo genero = GeneroJogo.Acao)
    {
        var adminToken = await ObterTokenAdminAsync();
        AutenticarComo(adminToken);
        var dto = new CriarJogoRequestDto(titulo ?? UniqueTitle(), "Descrição do jogo", genero, 29.90m, new DateTime(2024, 1, 1));
        var response = await Client.PostAsJsonAsync("/api/v1/jogos", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JogoResponseDto>())!;
    }

    [Fact]
    public async Task Criar_DeveRetornar201_QuandoAdmin()
    {
        var adminToken = await ObterTokenAdminAsync();
        AutenticarComo(adminToken);
        var titulo = UniqueTitle("Novo");
        var dto = new CriarJogoRequestDto(titulo, "Desc", GeneroJogo.RPG, 49.90m, DateTime.Now);

        var response = await Client.PostAsJsonAsync("/api/v1/jogos", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<JogoResponseDto>();
        result!.Titulo.Should().Be(titulo);
        result.Preco.Should().Be(49.90m);
    }

    [Fact]
    public async Task Criar_DeveRetornar403_QuandoUsuarioComum()
    {
        var token = await ObterTokenUsuarioAsync();
        AutenticarComo(token);
        var dto = new CriarJogoRequestDto(UniqueTitle(), "Desc", GeneroJogo.Acao, 10, DateTime.Now);

        var response = await Client.PostAsJsonAsync("/api/v1/jogos", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Criar_DeveRetornar409_QuandoTituloDuplicado()
    {
        var titulo = UniqueTitle("Dup");
        await CriarJogoAsync(titulo);
        var dto = new CriarJogoRequestDto(titulo, "Desc", GeneroJogo.Acao, 10, DateTime.Now);

        var response = await Client.PostAsJsonAsync("/api/v1/jogos", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Listar_DeveRetornar200_QuandoAutenticado()
    {
        await CriarJogoAsync();
        var token = await ObterTokenUsuarioAsync();
        AutenticarComo(token);

        var response = await Client.GetAsync("/api/v1/jogos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginacaoResponseDto<JogoResponseDto>>();
        result!.Itens.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Listar_DeveRetornar200_QuandoSemAuth()
    {
        await CriarJogoAsync();

        var response = await Client.GetAsync("/api/v1/jogos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginacaoResponseDto<JogoResponseDto>>();
        result!.Itens.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ListarPorGenero_DeveRetornarFiltrado()
    {
        await CriarJogoAsync(genero: GeneroJogo.RPG);
        await CriarJogoAsync(genero: GeneroJogo.Acao);

        var response = await Client.GetAsync("/api/v1/jogos?genero=2"); // RPG

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginacaoResponseDto<JogoResponseDto>>();
        result!.Itens.Should().AllSatisfy(j => j.Genero.Should().Be(GeneroJogo.RPG));
    }

    [Fact]
    public async Task ObterPorId_DeveRetornar200_QuandoSemAuth()
    {
        var jogo = await CriarJogoAsync();

        var response = await Client.GetAsync($"/api/v1/jogos/{jogo.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<JogoResponseDto>();
        result!.Id.Should().Be(jogo.Id);
    }

    [Fact]
    public async Task Atualizar_DeveRetornar200_QuandoAdmin()
    {
        var jogo = await CriarJogoAsync();
        var novoTitulo = UniqueTitle("Updated");
        var dto = new AtualizarJogoRequestDto(novoTitulo, "Nova desc", GeneroJogo.RPG, 39.90m);

        var response = await Client.PutAsJsonAsync($"/api/v1/jogos/{jogo.Id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<JogoResponseDto>();
        result!.Titulo.Should().Be(novoTitulo);
    }

    [Fact]
    public async Task Remover_DeveRetornar204_QuandoAdmin()
    {
        var jogo = await CriarJogoAsync();

        var response = await Client.DeleteAsync($"/api/v1/jogos/{jogo.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
