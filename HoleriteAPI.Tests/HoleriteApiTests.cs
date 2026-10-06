using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HoleriteAPI.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HoleriteAPI.Tests;

public class HoleriteApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  private const string Completo =
    "SalarioBruto=6000&HorasNoturnas=00:00&HorasExtras75=00:00&HorasExtras100=00:00" +
    "&DiasUteis=22&DomingosFeriados=5&PlanoMedico=0&OutrosDescontos=0";

  [Fact]
  public async Task Get_ComPedidoValido_Devolve200ComTotais()
  {
    HttpResponseMessage resposta = await _client.GetAsync($"/api/holerite?{Completo}");

    resposta.StatusCode.Should().Be(HttpStatusCode.OK);
    HoleriteResponse? corpo = await resposta.Content.ReadFromJsonAsync<HoleriteResponse>();
    corpo!.Dados.TotalINSS.Should().Be(641.51m);
    corpo.Dados.TotalIRRF.Should().Be(385.10m);
  }

  [Theory]
  [InlineData("SalarioBruto=0&DiasUteis=22&DomingosFeriados=5")]                       // salário zerado
  [InlineData("DiasUteis=22&DomingosFeriados=5")]                                       // sem salário
  [InlineData("SalarioBruto=3000&DiasUteis=0&DomingosFeriados=0")]                      // dias úteis zerados (antes dava 500)
  [InlineData("SalarioBruto=3000&DiasUteis=22&DomingosFeriados=5&HorasNoturnas=abc")]   // horário inválido
  [InlineData("SalarioBruto=3000&DiasUteis=22&DomingosFeriados=5&HorasExtras75=2:75")]  // minutos inválidos
  [InlineData("SalarioBruto=3000&DiasUteis=22&DomingosFeriados=5&PlanoMedico=-10")]     // desconto negativo
  public async Task Get_ComPedidoInvalido_Devolve400(string query)
  {
    HttpResponseMessage resposta = await _client.GetAsync($"/api/holerite?{query}");

    resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}
