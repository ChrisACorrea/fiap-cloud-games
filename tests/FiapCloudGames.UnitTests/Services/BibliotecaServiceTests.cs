using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.Exceptions;
using FiapCloudGames.Domain.Repositories;
using FiapCloudGames.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace FiapCloudGames.UnitTests.Services;

public class BibliotecaServiceTests
{
    private readonly Mock<IBibliotecaRepository> _bibliotecaMock = new();
    private readonly Mock<IJogoRepository> _jogoMock = new();
    private readonly Mock<IUsuarioRepository> _usuarioMock = new();
    private readonly BibliotecaService _service;

    public BibliotecaServiceTests()
    {
        _service = new BibliotecaService(_bibliotecaMock.Object, _jogoMock.Object, _usuarioMock.Object);
    }

    private static Usuario CriarUsuarioAtivo() =>
        new("Felipe", new Email("felipe@email.com"), "hash");

    private static Jogo CriarJogoAtivo() =>
        new("Jogo Teste", "Descrição", GeneroJogo.RPG, new Preco(59.90m), DateTime.Now);

    [Fact]
    public async Task AdquirirJogoAsync_DeveAdicionarJogo_QuandoDadosValidos()
    {
        var usuario = CriarUsuarioAtivo();
        var jogo = CriarJogoAtivo();

        _usuarioMock.Setup(r => r.ObterPorIdAsync("usuario-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _jogoMock.Setup(r => r.ObterPorIdAsync("jogo-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(jogo);
        _bibliotecaMock.Setup(r => r.UsuarioPossuiJogoAsync("usuario-id", "jogo-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _service.AdquirirJogoAsync("usuario-id", "jogo-id");

        _bibliotecaMock.Verify(r => r.AdicionarJogoAsync(It.IsAny<BibliotecaJogo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AdquirirJogoAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        _usuarioMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var act = () => _service.AdquirirJogoAsync("id", "jogo-id");

        await act.Should().ThrowAsync<EntidadeNaoEncontradaException>();
    }

    [Fact]
    public async Task AdquirirJogoAsync_DeveLancarExcecao_QuandoUsuarioInativo()
    {
        var usuario = CriarUsuarioAtivo();
        usuario.Desativar();

        _usuarioMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var act = () => _service.AdquirirJogoAsync("id", "jogo-id");

        await act.Should().ThrowAsync<ValidacaoException>()
            .WithMessage("*inativo*");
    }

    [Fact]
    public async Task AdquirirJogoAsync_DeveLancarExcecao_QuandoJogoInativo()
    {
        var usuario = CriarUsuarioAtivo();
        var jogo = CriarJogoAtivo();
        jogo.Desativar();

        _usuarioMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _jogoMock.Setup(r => r.ObterPorIdAsync("jogo-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(jogo);

        var act = () => _service.AdquirirJogoAsync("id", "jogo-id");

        await act.Should().ThrowAsync<ValidacaoException>()
            .WithMessage("*inativo*");
    }

    [Fact]
    public async Task AdquirirJogoAsync_DeveLancarExcecao_QuandoJogoJaPossuido()
    {
        var usuario = CriarUsuarioAtivo();
        var jogo = CriarJogoAtivo();

        _usuarioMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _jogoMock.Setup(r => r.ObterPorIdAsync("jogo-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(jogo);
        _bibliotecaMock.Setup(r => r.UsuarioPossuiJogoAsync("id", "jogo-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _service.AdquirirJogoAsync("id", "jogo-id");

        await act.Should().ThrowAsync<ConflitoDeDadosException>();
    }

    [Fact]
    public async Task ListarBibliotecaAsync_DeveRetornarJogos()
    {
        var usuario = CriarUsuarioAtivo();
        var jogo = CriarJogoAtivo();

        _usuarioMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _bibliotecaMock.Setup(r => r.ObterPorUsuarioAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new BibliotecaJogo("id", "jogo-id")]);
        _jogoMock.Setup(r => r.ObterPorIdAsync("jogo-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(jogo);

        var result = await _service.ListarBibliotecaAsync("id");

        result.Should().HaveCount(1);
    }
}
