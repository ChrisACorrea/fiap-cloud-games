using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace FiapCloudGames.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Jogo> Jogos => Set<Jogo>();
    public DbSet<BibliotecaJogo> BibliotecaJogos => Set<BibliotecaJogo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        GenerateObjectIds();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void GenerateObjectIds()
    {
        var addedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && !e.Metadata.IsOwned());

        foreach (var entry in addedEntries)
        {
            var idProperty = entry.Property("Id");
            if (idProperty.CurrentValue is string id && string.IsNullOrEmpty(id))
            {
                idProperty.CurrentValue = ObjectId.GenerateNewId().ToString();
            }
        }
    }
}
