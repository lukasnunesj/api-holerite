namespace HoleriteAPI.Core.Domain
{
  public record FaixaIRRF(decimal Ate, decimal Aliquota, decimal Deducao);

  /// <param name="DescontoSimplificado">Alternativa às deduções legais (INSS), usada quando for mais vantajosa.</param>
  /// <param name="ReducaoIsencao">Redução fixa até <paramref name="LimiteIsencao"/>, o que zera o imposto.</param>
  /// <param name="LimiteReducao">Acima disso não há redução.</param>
  /// <param name="ReducaoBase">Entre os dois limites: redução = ReducaoBase - ReducaoFator x rendimento.</param>
  public record TabelaIRRF(
    IReadOnlyList<FaixaIRRF> Faixas,
    decimal DescontoSimplificado,
    decimal LimiteIsencao,
    decimal ReducaoIsencao,
    decimal LimiteReducao,
    decimal ReducaoBase,
    decimal ReducaoFator
  );

  public static class IRRF
  {
    /// <summary>
    /// IRRF mensal sem dependentes. A base é o rendimento menos a dedução mais vantajosa (INSS ou
    /// desconto simplificado). O imposto sai da tabela progressiva e depois recebe a redução da
    /// Lei 15.270/2025, calculada sobre o rendimento bruto, nunca sobre a base.
    /// </summary>
    public static decimal Calcular(decimal rendimento, decimal inss, TabelaIRRF tabela)
    {
      decimal baseCalculo = Math.Max(0, rendimento - Math.Max(inss, tabela.DescontoSimplificado));
      FaixaIRRF faixa = tabela.Faixas.First(f => baseCalculo <= f.Ate);

      decimal imposto = baseCalculo * faixa.Aliquota - faixa.Deducao;

      return Dinheiro.Arredondar(Math.Max(0, imposto - Reducao(rendimento, tabela)));
    }

    private static decimal Reducao(decimal rendimento, TabelaIRRF tabela)
    {
      if (rendimento <= tabela.LimiteIsencao) return tabela.ReducaoIsencao;
      if (rendimento <= tabela.LimiteReducao) return tabela.ReducaoBase - tabela.ReducaoFator * rendimento;
      return 0;
    }
  }
}
