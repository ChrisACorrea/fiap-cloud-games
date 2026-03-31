using System.Net;
using System.Net.Http.Json;
using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.IntegrationTests.Fixtures;
using FluentAssertions;

namespace FiapCloudGames.IntegrationTests.Controllers;

public sealed class UsuariosControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Criar_DeveRetornar201_QuandoDadosValidos()
    {
        var email = $"novo-{Guid.NewGuid():N}@test.com";
        var dto = new CriarUsuarioRequestDto("Novo User", email, "Senha@123");

        var response = await Client.PostAsJsonAsync("/api/v1/usuarios", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<UsuarioResponseDto>();
        result!.Nome.Should().Be("Novo User");
        result.Email.Should().Be(email);
    }

    [Fact]
    public async Task Criar_DeveRetornar409_QuandoEmailDuplicado()
    {
        var email = $"dup-{Guid.NewGuid():N}@test.com";
        await RegistrarUsuarioAsync("User 1", email, "Senha@123");

        var dto = new CriarUsuarioRequestDto("User 2", email, "Senha@123");
        var response = await Client.PostAsJsonAsync("/api/v1/usuarios", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Criar_DeveRetornar422_QuandoDadosInvalidos()
    {
        var dto = new CriarUsuarioRequestDto("", "", "curta");

        var response = await Client.PostAsJsonAsync("/api/v1/usuarios", dto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Listar_DeveRetornar200_QuandoAdmin()
    {
        var token = await ObterTokenAdminAsync();
        AutenticarComo(token);

        var response = await Client.GetAsync("/api/v1/usuarios");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginacaoResponseDto<UsuarioResponseDto>>();
        result!.Itens.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Listar_DeveRetornar403_QuandoUsuarioComum()
    {
        var token = await ObterTokenUsuarioAsync();
        AutenticarComo(token);

        var response = await Client.GetAsync("/api/v1/usuarios");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Listar_DeveRetornar401_QuandoSemAuth()
    {
        var response = await Client.GetAsync("/api/v1/usuarios");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornar200_QuandoProprioUsuario()
    {
        var email = $"self-{Guid.NewGuid():N}@test.com";
        var dto = new CriarUsuarioRequestDto("Self User", email, "Senha@123");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/usuarios", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<UsuarioResponseDto>();
        var token = await LoginAsync(email, "Senha@123");
        AutenticarComo(token);

        var response = await Client.GetAsync($"/api/v1/usuarios/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Remover_DeveRetornar204_QuandoAdmin()
    {
        // Criar usuário para deletar
        var email = $"del-{Guid.NewGuid():N}@test.com";
        var createResponse = await Client.PostAsJsonAsync("/api/v1/usuarios",
            new CriarUsuarioRequestDto("Para Deletar", email, "Senha@123"));
        var created = await createResponse.Content.ReadFromJsonAsync<UsuarioResponseDto>();

        var adminToken = await ObterTokenAdminAsync();
        AutenticarComo(adminToken);

        var response = await Client.DeleteAsync($"/api/v1/usuarios/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
