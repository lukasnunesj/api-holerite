using FluentAssertions;
using HoleriteAPI.Core.Domain;
using HoleriteAPI.Data.Repositories;

namespace HoleriteAPI.Tests;

public class INSSTests
{
  // Faixas de 2025: a Receita publica exemplos oficiais (Lei 15.270) calculados com elas.
  private static readonly FaixaINSS[] Faixas2025 =
  [
    new(1518.00m, 0.075m),
    new(2793.88m, 0.09m),
    new(4190.83m, 0.12m),
    new(8157.41m, 0.14m),
  ];

  private readonly IReadOnlyList<FaixaINSS> _faixas2026 = new HoleriteRepository().CarregarFaixasINSS();

  [Theory]
  [InlineData(3036.00, 257.73)]
  [InlineData(4000.00, 373.41)]
  [InlineData(5000.00, 509.60)]
  [InlineData(6000.00, 649.60)]
  public void Calcular_ComFaixas2025_BateComExemplosOficiais(double salario, double esperado)
  {
    INSS.Calcular((decimal)salario, Faixas2025).Should().Be((decimal)esperado);
  }

  [Theory]
  [InlineData(1621.00, 121.58)] // 7,5% da primeira faixa inteira: 121,575 arredonda para cima
  [InlineData(3000.00, 248.60)]
  [InlineData(6000.00, 641.51)]
  public void Calcular_Faixas2026_AplicaAliquotaSoNaParteDeCadaFaixa(double salario, double esperado)
  {
    INSS.Calcular((decimal)salario, _faixas2026).Should().Be((decimal)esperado);
  }

  [Theory]
  [InlineData(8475.55)]
  [InlineData(12000.00)]
  [InlineData(50000.00)]
  public void Calcular_NoTetoOuAcima_NaoPassaDaContribuicaoMaxima(double salario)
  {
    INSS.Calcular((decimal)salario, _faixas2026).Should().Be(988.09m);
  }

  [Fact]
  public void Calcular_SalarioZero_NaoContribui()
  {
    INSS.Calcular(0m, _faixas2026).Should().Be(0m);
  }
}
