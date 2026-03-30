using System.Text.RegularExpressions;
using FiapCloudGames.Domain.Exceptions;

namespace FiapCloudGames.Domain.ValueObjects;

public sealed partial record Email
{
    public string Endereco { get; }

    public Email(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
        {
            throw new ValidacaoException("O e-mail é obrigatório.");
        }

        endereco = endereco.Trim().ToLowerInvariant();

        if (!EmailRegex().IsMatch(endereco))
        {
            throw new ValidacaoException("O formato do e-mail é inválido.");
        }

        Endereco = endereco;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    public override string ToString() => Endereco;
}
