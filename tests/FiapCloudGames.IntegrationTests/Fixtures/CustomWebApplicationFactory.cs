using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;

namespace FiapCloudGames.IntegrationTests.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private MongoDbContainer? _mongoContainer;
    private string _connectionString = null!;

    public string DatabaseName { get; } = $"fcg_test_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDbSettings:ConnectionString"] = _connectionString,
                ["MongoDbSettings:DatabaseName"] = DatabaseName,
                ["JwtSettings:SecretKey"] = "FiapCloudGames_Dev_SecretKey_Com_Pelo_Menos_256_Bits_Para_HMAC_SHA256!",
                ["JwtSettings:Issuer"] = "FiapCloudGames",
                ["JwtSettings:Audience"] = "FiapCloudGames",
                ["JwtSettings:ExpiracaoEmMinutos"] = "30"
            });
        });

        builder.UseEnvironment("Development");
    }

    public async Task InitializeAsync()
    {
        var ciMongo = Environment.GetEnvironmentVariable("INTEGRATION_TEST_MONGODB");

        if (!string.IsNullOrEmpty(ciMongo))
        {
            // CI: usa MongoDB service container provido pelo GitHub Actions
            _connectionString = ciMongo;
        }
        else
        {
            // Local: usa Testcontainers para criar MongoDB temporario
            _mongoContainer = new MongoDbBuilder("mongo:7.0").Build();
            await _mongoContainer.StartAsync();
            _connectionString = _mongoContainer.GetConnectionString();
        }
    }

    public new async Task DisposeAsync()
    {
        if (_mongoContainer is not null)
            await _mongoContainer.DisposeAsync();

        await base.DisposeAsync();
    }
}
