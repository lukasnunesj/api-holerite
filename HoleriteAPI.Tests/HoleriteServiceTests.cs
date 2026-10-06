using FluentAssertions;
using HoleriteAPI.Core.Application;
using HoleriteAPI.Core.Application.DTOs;
using HoleriteAPI.Core.Application.Requests;
using HoleriteAPI.Data.Repositories;

namespace HoleriteAPI.Tests;

public class HoleriteServiceTests
{
  private readonly HoleriteService _service = new(new HoleriteRepository());

  private static HoleriteRequest Pedido(
    decimal salario,
    string noturnas = "00:00",
    string extras75 = "00:00",
    string extras100 = "00:00",
    decimal planoMedico = 0,
    decimal outrosDescontos = 0) => new()
  {
    SalarioBruto = salario,
    HorasNoturnas = noturnas,
    HorasExtras75 = extras75,
    HorasExtras100 = extras100,
    DiasUteis = 22,
    DomingosFeriados = 5,
    PlanoMedico = planoMedico,
    OutrosDescontos = outrosDescontos,
  };

  [Fact]
  public void CalculaTotais_NaoTrocaINSSComIRRF()
  {
    // Regressão: a API devolvia o INSS no campo do IRRF e vice-versa.
    HoleriteResponseDTO r = _service.CalculaTotais(Pedido(6000m));

    r.TotalINSS.Should().Be(641.51m);
    r.TotalIRRF.Should().Be(385.10m);
    r.ValorValeAdiantamento.Should().Be(2400m);
    r.TotalDebitos.Should().Be(6000m);
    r.TotalGeral.Should().Be(2573.39m); // 6.000 - 2.400 (vale) - 641,51 - 385,10
  }

  [Fact]
  public void CalculaTotais_ComAdicionalNoturnoEHorasExtras75()
  {
    // valor da hora = 3.000 / 200 = 15
    HoleriteResponseDTO r = _service.CalculaTotais(Pedido(3000m, noturnas: "10:00", extras75: "02:00"));

    r.TotalAdicionalNoturno.Should().Be(45.00m);   // 10 h x 15 x 30%
    r.TotalHorasExtras75.Should().Be(52.50m);      // 2 h x 15 x 1,75
    r.TotalHorasExtras100.Should().Be(0m);
    r.TotalDSRNoturno.Should().Be(10.23m);         // 45 / 22 x 5
    r.TotalDSRHoraExtra.Should().Be(11.93m);       // 52,50 / 22 x 5
    r.TotalDebitos.Should().Be(3119.66m);
    r.TotalINSS.Should().Be(262.96m);
    r.TotalIRRF.Should().Be(0m);                   // até R$ 5.000 o imposto é zerado
    r.TotalGeral.Should().Be(1656.70m);            // 3.119,66 - 1.200 (vale) - 262,96
  }

  [Fact]
  public void CalculaTotais_ComHorasExtras100_PlanoMedicoEOutrosDescontos()
  {
    // valor da hora = 2.000 / 200 = 10
    HoleriteResponseDTO r = _service.CalculaTotais(
      Pedido(2000m, extras100: "01:30", planoMedico: 100m, outrosDescontos: 50.50m));

    r.TotalHorasExtras100.Should().Be(30.00m);     // 1,5 h x 10 x 2
    r.TotalDSRHoraExtra.Should().Be(6.82m);        // 30 / 22 x 5
    r.TotalDebitos.Should().Be(2036.82m);
    r.TotalINSS.Should().Be(159.00m);
    r.TotalGeral.Should().Be(927.32m);             // 2.036,82 - 800 - 100 - 50,50 - 159
  }
}
