using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.Exceptions;
using FiapCloudGames.Domain.Repositories;
using FiapCloudGames.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace FiapCloudGames.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _repositoryMock = new();
    private readonly JogoService _service;

    public JogoServiceTests()
    {
        _service = new JogoService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarJogo_QuandoDadosValidos()
    {
        var dto = new CriarJogoRequestDto("Novo Jogo", "Descrição", GeneroJogo.RPG, 59.90m, DateTime.Now);
        _repositoryMock.Setup(r => r.TituloExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.CriarAsync(dto);

        result.Titulo.Should().Be("Novo Jogo");
        result.Genero.Should().Be(GeneroJogo.RPG);
        result.Preco.Should().Be(59.90m);
        _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoTituloJaExiste()
    {
        var dto = new CriarJogoRequestDto("Existente", "Desc", GeneroJogo.Acao, 10, DateTime.Now);
        _repositoryMock.Setup(r => r.TituloExisteAsync("Existente", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _service.CriarAsync(dto);

        await act.Should().ThrowAsync<ConflitoDeDadosException>();
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveLancarExcecao_QuandoNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Jogo?)null);

        var act = () => _service.ObterPorIdAsync("id");

        await act.Should().ThrowAsync<EntidadeNaoEncontradaException>();
    }

    [Fact]
    public async Task RemoverAsync_DeveDesativarJogo()
    {
        var jogo = new Jogo("Título", "Desc", GeneroJogo.Acao, new Preco(10), DateTime.Now);
        _repositoryMock.Setup(r => r.ObterPorIdAsync("id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(jogo);

        await _service.RemoverAsync("id");

        jogo.Ativo.Should().BeFalse();
        _repositoryMock.Verify(r => r.AtualizarAsync(jogo, It.IsAny<CancellationToken>()), Times.Once);
    }
}
