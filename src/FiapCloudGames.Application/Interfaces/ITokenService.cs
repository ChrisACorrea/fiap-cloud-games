using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Application.Interfaces;

public interface ITokenService
{
    string GerarToken(Usuario usuario);
    DateTime ObterExpiracao();
}
