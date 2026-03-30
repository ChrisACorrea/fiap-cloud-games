using FiapCloudGames.Domain.Exceptions;

namespace FiapCloudGames.Domain.ValueObjects;

public sealed record Preco
{
    public decimal Valor { get; }
    public string Moeda { get; }

    public Preco(decimal valor, string moeda = "BRL")
    {
        if (valor < 0)
        {
            throw new ValidacaoException("O preço deve ser um valor positivo.");
        }

        if (string.IsNullOrWhiteSpace(moeda))
        {
            throw new ValidacaoException("A moeda é obrigatória.");
        }

        Valor = valor;
        Moeda = moeda.Trim().ToUpperInvariant();
    }

    public override string ToString() => $"{Moeda} {Valor:N2}";
}
