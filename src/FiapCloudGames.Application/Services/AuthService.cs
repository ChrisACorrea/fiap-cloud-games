using FiapCloudGames.Application.DTOs.Request;
using FiapCloudGames.Application.DTOs.Response;
using FiapCloudGames.Application.Interfaces;
using FiapCloudGames.Domain.Exceptions;
using FiapCloudGames.Domain.Repositories;

namespace FiapCloudGames.Application.Services;

public sealed class AuthService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.ObterPorEmailAsync(dto.Email.Trim().ToLowerInvariant(), ct);

        if (usuario is null || !passwordHasher.Verify(dto.Senha, usuario.SenhaHash))
        {
            throw new CredenciaisInvalidasException();
        }

        if (!usuario.Ativo)
        {
            throw new CredenciaisInvalidasException("Conta desativada. Entre em contato com o suporte.");
        }

        var token = tokenService.GerarToken(usuario);
        var expiracao = tokenService.ObterExpiracao();

        return new LoginResponseDto(token, expiracao, usuario.Tipo);
    }
}
