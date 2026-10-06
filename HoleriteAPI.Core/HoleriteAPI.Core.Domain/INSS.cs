namespace HoleriteAPI.Core.Domain
{
  public record FaixaINSS(decimal Ate, decimal Aliquota);

  public static class INSS
  {
    /// <summary>
    /// Contribuição progressiva: cada alíquota incide só sobre a parte do salário dentro da sua faixa.
    /// O que passa do teto (limite da última faixa) não contribui.
    /// </summary>
    public static decimal Calcular(decimal salario, IReadOnlyList<FaixaINSS> faixas)
    {
      decimal total = 0;
      decimal limiteAnterior = 0;

      foreach (FaixaINSS faixa in faixas)
      {
        if (salario <= limiteAnterior) break;

        total += (Math.Min(salario, faixa.Ate) - limiteAnterior) * faixa.Aliquota;
        limiteAnterior = faixa.Ate;
      }

      return Dinheiro.Arredondar(total);
    }
  }
}
