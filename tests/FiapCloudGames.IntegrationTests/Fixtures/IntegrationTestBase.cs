using System.Net.Http.Headers;
using System.Net.Http.Json;
using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;

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
        // Usa o admin criado pelo DatabaseSeed na startup (admin@fcg.com / Admin@123456)
        return await LoginAsync("admin@fcg.com", "Admin@123456");
    }

    protected async Task<string> ObterTokenUsuarioAsync(string? email = null)
    {
        email ??= $"user-{Guid.NewGuid():N}@test.com";
        await RegistrarUsuarioAsync("Usuario Teste", email, "Senha@123");
        return await LoginAsync(email, "Senha@123");
    }

    protected async Task RegistrarUsuarioAsync(string nome, string email, string senha)
    {
        var dto = new CriarUsuarioRequestDto(nome, email, senha);
        var response = await Client.PostAsJsonAsync("/api/v1/usuarios", dto);
        response.EnsureSuccessStatusCode();
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

    protected void Desautenticar()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}

[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<CustomWebApplicationFactory>;
