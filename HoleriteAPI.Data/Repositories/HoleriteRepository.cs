using HoleriteAPI.Core.Domain;
using HoleriteAPI.Core.Domain.Ports;

namespace HoleriteAPI.Data.Repositories
{
  /// <summary>
  /// Tabelas vigentes em 2026. Atualizar todo janeiro.
  /// INSS: Portaria Interministerial MPS/MF nº 13/2026.
  /// IRRF: Receita Federal, "Tributação de 2026" (tabela progressiva mensal e desconto simplificado)
  /// e Lei 15.270/2025 (redução do imposto).
  /// </summary>
  public class HoleriteRepository : IHoleriteRepository
  {
    public IReadOnlyList<FaixaINSS> CarregarFaixasINSS() =>
    [
      new(1621.00m, 0.075m),
      new(2902.84m, 0.09m),
      new(4354.27m, 0.12m),
      new(8475.55m, 0.14m),
    ];

    public TabelaIRRF CarregarTabelaIRRF() => new(
      Faixas:
      [
        new(2428.80m, 0m, 0m),
        new(2826.65m, 0.075m, 182.16m),
        new(3751.05m, 0.15m, 394.16m),
        new(4664.68m, 0.225m, 675.49m),
        new(decimal.MaxValue, 0.275m, 908.73m),
      ],
      DescontoSimplificado: 607.20m,
      LimiteIsencao: 5000.00m,
      ReducaoIsencao: 312.89m,
      LimiteReducao: 7350.00m,
      ReducaoBase: 978.62m,
      ReducaoFator: 0.133145m
    );
  }
}
