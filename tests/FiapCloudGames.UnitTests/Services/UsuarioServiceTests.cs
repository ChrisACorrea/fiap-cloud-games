using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.Interfaces;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.Exceptions;
using FiapCloudGames.Domain.Repositories;
using FiapCloudGames.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace FiapCloudGames.UnitTests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _service = new UsuarioService(_repositoryMock.Object, _hasherMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarUsuario_QuandoDadosValidos()
    {
        var dto = new CriarUsuarioRequestDto("Felipe", "felipe@email.com", "Senha@123");
        _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _hasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");

        var result = await _service.CriarAsync(dto);

        result.Nome.Should().Be("Felipe");
        result.Email.Should().Be("felipe@email.com");
        result.Tipo.Should().Be(TipoUsuario.Usuario);
        _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoEmailJaExiste()
    {
        var dto = new CriarUsuarioRequestDto("Felipe", "existente@email.com", "Senha@123");
        _repositoryMock.Setup(r => r.EmailExisteAsync("existente@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _service.CriarAsync(dto);

        await act.Should().ThrowAsync<ConflitoDeDadosException>();
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveLancarExcecao_QuandoNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync("id-inexistente", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var act = () => _service.ObterPorIdAsync("id-inexistente");

        await act.Should().ThrowAsync<EntidadeNaoEncontradaException>();
    }

    [Fact]
    public async Task RemoverAsync_DeveDesativarUsuario()
    {
        var usuario = new Usuario("Felipe", new Email("felipe@email.com"), "hash");
        _repositoryMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        await _service.RemoverAsync("id");

        usuario.Ativo.Should().BeFalse();
        _repositoryMock.Verify(r => r.AtualizarAsync(usuario, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoEmailJaExisteEmOutroUsuario()
    {
        var usuario = new Usuario("Felipe", new Email("felipe@email.com"), "hash");
        _repositoryMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _repositoryMock.Setup(r => r.EmailExisteAsync("outro@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = new AtualizarUsuarioRequestDto("Felipe", "outro@email.com");
        var act = () => _service.AtualizarAsync("id", dto);

        await act.Should().ThrowAsync<ConflitoDeDadosException>();
    }
}
