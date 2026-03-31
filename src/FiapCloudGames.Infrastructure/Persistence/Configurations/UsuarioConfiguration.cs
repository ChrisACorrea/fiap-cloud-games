using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FiapCloudGames.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToCollection("usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Endereco,
                endereco => new Email(endereco))
            .IsRequired();

        builder.Property(u => u.SenhaHash)
            .IsRequired();

        builder.Property(u => u.Tipo)
            .IsRequired();

        builder.Property(u => u.DataCriacao)
            .IsRequired();

        builder.Property(u => u.Ativo)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
