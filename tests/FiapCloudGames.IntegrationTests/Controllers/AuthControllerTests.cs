using System.Net;
using System.Net.Http.Json;
using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.IntegrationTests.Controllers;

public sealed class AuthControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var email = $"auth-{Guid.NewGuid():N}@test.com";
        await RegistrarUsuarioAsync("Teste", email, "Senha@123");

        var dto = new LoginRequestDto(email, "Senha@123");
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Expiracao.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_DeveRetornar401_QuandoCredenciaisInvalidas()
    {
        var dto = new LoginRequestDto("naoexiste@test.com", "Senha@123");

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_DeveRetornar422_QuandoDadosInvalidos()
    {
        var dto = new LoginRequestDto("", "");

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(422);
    }
}
