using System.Net.Http.Headers;
using System.Net.Http.Json;
using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Application.Interfaces;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.ValueObjects;
using FiapCloudGames.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.IntegrationTests.Fixtures;

[Collection("Integration")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected HttpClient Client { get; private set; } = null!;

    protected IntegrationTestBase(CustomWebApplicationFactory factory) => Factory = factory;

    public Task InitializeAsync()
    {
        Client = Factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected async Task<string> ObterTokenAdminAsync()
    {
        var email = $"admin-{Guid.NewGuid():N}@test.com";
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var senhaHash = hasher.Hash("Admin@123456");
        var admin = new Usuario("Admin Teste", new Email(email), senhaHash, TipoUsuario.Administrador);
        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();

        return await LoginAsync(email, "Admin@123456");
    }

    protected async Task<string> ObterTokenUsuarioAsync(string? email = null)
    {
        email ??= $"user-{Guid.NewGuid():N}@test.com";
        await RegistrarUsuarioAsync("Usuário Teste", email, "Senha@123");
        return await LoginAsync(email, "Senha@123");
    }

    protected async Task RegistrarUsuarioAsync(string nome, string email, string senha)
    {
        var dto = new CriarUsuarioRequestDto(nome, email, senha);
        await Client.PostAsJsonAsync("/api/v1/usuarios", dto);
    }

    protected async Task<string> LoginAsync(string email, string senha)
    {
        var dto = new LoginRequestDto(email, senha);
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", dto);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return result!.Token;
    }

    protected void AutenticarComo(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected void RemoverAutenticacao()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}

[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<CustomWebApplicationFactory>;
