using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;

namespace FiapCloudGames.IntegrationTests.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;

    public string DatabaseName { get; } = $"fcg_test_{Guid.NewGuid():N}";

    public CustomWebApplicationFactory()
    {
        var orbstackSocket = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".orbstack", "run", "docker.sock");
        if (File.Exists(orbstackSocket) && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOCKER_HOST")))
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", $"unix://{orbstackSocket}");
        }

        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDbSettings:ConnectionString"] = _mongoContainer.GetConnectionString(),
                ["MongoDbSettings:DatabaseName"] = DatabaseName,
                ["JwtSettings:SecretKey"] = "FiapCloudGames_Dev_SecretKey_Com_Pelo_Menos_256_Bits_Para_HMAC_SHA256!",
                ["JwtSettings:Issuer"] = "FiapCloudGames",
                ["JwtSettings:Audience"] = "FiapCloudGames",
                ["JwtSettings:ExpiracaoEmMinutos"] = "30"
            });
        });

        builder.UseEnvironment("Development");
    }

    public async Task InitializeAsync() => await _mongoContainer.StartAsync();

    public new async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
