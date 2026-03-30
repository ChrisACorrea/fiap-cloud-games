using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.Exceptions;
using FiapCloudGames.Domain.Repositories;
using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Application.Services;

public sealed class JogoService(IJogoRepository jogoRepository) : IJogoService
{
    public async Task<JogoResponseDto> CriarAsync(CriarJogoRequestDto dto, CancellationToken ct = default)
    {
        if (await jogoRepository.TituloExisteAsync(dto.Titulo, ct))
        {
            throw new ConflitoDeDadosException("Jogo", "título", dto.Titulo);
        }

        var preco = new Preco(dto.Preco);
        var jogo = new Jogo(dto.Titulo, dto.Descricao, dto.Genero, preco, dto.DataLancamento);

        await jogoRepository.CriarAsync(jogo, ct);

        return MapToDto(jogo);
    }

    public async Task<JogoResponseDto> ObterPorIdAsync(string id, CancellationToken ct = default)
    {
        var jogo = await jogoRepository.ObterPorIdAsync(id, ct)
            ?? throw new EntidadeNaoEncontradaException("Jogo", id);

        return MapToDto(jogo);
    }

    public async Task<PaginacaoResponseDto<JogoResponseDto>> ListarAsync(int pagina, int tamanhoPagina, GeneroJogo? genero, CancellationToken ct = default)
    {
        var jogos = genero.HasValue
            ? await jogoRepository.BuscarPorGeneroAsync(genero.Value, pagina, tamanhoPagina, ct)
            : await jogoRepository.ObterTodosAsync(pagina, tamanhoPagina, ct);

        var itens = jogos.Select(MapToDto);

        return new PaginacaoResponseDto<JogoResponseDto>(itens, pagina, tamanhoPagina, itens.Count());
    }

    public async Task<JogoResponseDto> AtualizarAsync(string id, AtualizarJogoRequestDto dto, CancellationToken ct = default)
    {
        var jogo = await jogoRepository.ObterPorIdAsync(id, ct)
            ?? throw new EntidadeNaoEncontradaException("Jogo", id);

        if (dto.Titulo != jogo.Titulo && await jogoRepository.TituloExisteAsync(dto.Titulo, ct))
        {
            throw new ConflitoDeDadosException("Jogo", "título", dto.Titulo);
        }

        var novoPreco = new Preco(dto.Preco);

        jogo.AtualizarTitulo(dto.Titulo);
        jogo.AtualizarDescricao(dto.Descricao);
        jogo.AtualizarGenero(dto.Genero);
        jogo.AtualizarPreco(novoPreco);

        await jogoRepository.AtualizarAsync(jogo, ct);

        return MapToDto(jogo);
    }

    public async Task RemoverAsync(string id, CancellationToken ct = default)
    {
        var jogo = await jogoRepository.ObterPorIdAsync(id, ct)
            ?? throw new EntidadeNaoEncontradaException("Jogo", id);

        jogo.Desativar();
        await jogoRepository.AtualizarAsync(jogo, ct);
    }

    private static JogoResponseDto MapToDto(Jogo jogo) =>
        new(
            jogo.Id,
            jogo.Titulo,
            jogo.Descricao,
            jogo.Genero,
            jogo.Preco.Valor,
            jogo.Preco.Moeda,
            jogo.DataLancamento,
            jogo.Ativo);
}
