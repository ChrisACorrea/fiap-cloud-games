using System.Text;
using FiapCloudGames.Application.Interfaces;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Repositories;
using FiapCloudGames.Infrastructure.Repositories;
using FiapCloudGames.Infrastructure.Services;
using FiapCloudGames.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace FiapCloudGames.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
                ?? throw new InvalidOperationException("MongoDbSettings não configurado.");

            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var settings = configuration.GetSection(MongoDbSettings.SectionName).Get<MongoDbSettings>()
                ?? throw new InvalidOperationException("MongoDbSettings não configurado.");

            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings não configurado.");

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApenasAdmin", policy =>
                policy.RequireRole("Administrador"));
            options.AddPolicy("UsuarioAutenticado", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IBibliotecaRepository, BibliotecaRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IBibliotecaService, BibliotecaService>();

        return services;
    }
}
