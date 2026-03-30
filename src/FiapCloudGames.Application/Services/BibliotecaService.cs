using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Exceptions;
using FiapCloudGames.Domain.Repositories;

namespace FiapCloudGames.Application.Services;

public sealed class BibliotecaService(
    IBibliotecaRepository bibliotecaRepository,
    IJogoRepository jogoRepository,
    IUsuarioRepository usuarioRepository) : IBibliotecaService
{
    public async Task AdquirirJogoAsync(string usuarioId, string jogoId, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new EntidadeNaoEncontradaException("Usuário", usuarioId);

        if (!usuario.Ativo)
        {
            throw new ValidacaoException("O usuário está inativo e não pode adquirir jogos.");
        }

        var jogo = await jogoRepository.ObterPorIdAsync(jogoId, ct)
            ?? throw new EntidadeNaoEncontradaException("Jogo", jogoId);

        if (!jogo.Ativo)
        {
            throw new ValidacaoException("O jogo está inativo e não pode ser adquirido.");
        }

        if (await bibliotecaRepository.UsuarioPossuiJogoAsync(usuarioId, jogoId, ct))
        {
            throw new ConflitoDeDadosException($"O usuário já possui o jogo '{jogo.Titulo}' em sua biblioteca.");
        }

        var bibliotecaJogo = new BibliotecaJogo(usuarioId, jogoId);
        await bibliotecaRepository.AdicionarJogoAsync(bibliotecaJogo, ct);
    }

    public async Task<IEnumerable<JogoResponseDto>> ListarBibliotecaAsync(string usuarioId, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.ObterPorIdAsync(usuarioId, ct)
            ?? throw new EntidadeNaoEncontradaException("Usuário", usuarioId);

        var bibliotecaJogos = await bibliotecaRepository.ObterPorUsuarioAsync(usuarioId, ct);

        var jogos = new List<JogoResponseDto>();
        foreach (var item in bibliotecaJogos)
        {
            var jogo = await jogoRepository.ObterPorIdAsync(item.JogoId, ct);
            if (jogo is not null)
            {
                jogos.Add(new JogoResponseDto(
                    jogo.Id,
                    jogo.Titulo,
                    jogo.Descricao,
                    jogo.Genero,
                    jogo.Preco.Valor,
                    jogo.Preco.Moeda,
                    jogo.DataLancamento,
                    jogo.Ativo));
            }
        }

        return jogos;
    }
}
