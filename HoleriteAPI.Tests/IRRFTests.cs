using FluentAssertions;
using HoleriteAPI.Core.Domain;
using HoleriteAPI.Data.Repositories;

namespace HoleriteAPI.Tests;

public class IRRFTests
{
  private readonly TabelaIRRF _tabela = new HoleriteRepository().CarregarTabelaIRRF();

  // Exemplos da Receita Federal: "Exemplos de Aplicação da Lei 15.270/2025".
  [Theory]
  [InlineData(3036.00, 257.73, 0.00)]    // desconto simplificado, base cai na faixa isenta
  [InlineData(4000.00, 373.41, 0.00)]    // imposto de 114,76 zerado pela redução
  [InlineData(5000.00, 509.60, 0.00)]    // imposto de 312,89 zerado pela redução
  [InlineData(6000.00, 649.60, 382.88)]  // 562,63 menos redução de 179,75, que usa o bruto, não a base
  [InlineData(7607.20, 0.00, 1016.27)]   // acima de 7.350 não há redução
  public void Calcular_BateComExemplosOficiaisDaReceita(double rendimento, double inss, double esperado)
  {
    IRRF.Calcular((decimal)rendimento, (decimal)inss, _tabela).Should().Be((decimal)esperado);
  }

  [Fact]
  public void Calcular_SemRedutor_UsaDeducaoMaisVantajosa()
  {
    // INSS (988,09) maior que o desconto simplificado: base = 10.000 - 988,09 = 9.011,91
    // imposto = 9.011,91 x 27,5% - 908,73 = 1.569,545...
    IRRF.Calcular(10000m, 988.09m, _tabela).Should().Be(1569.55m);
  }

  [Fact]
  public void Calcular_RendaBaixa_NuncaDevolveImpostoNegativo()
  {
    IRRF.Calcular(1000m, 75m, _tabela).Should().Be(0m);
  }
}
