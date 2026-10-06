namespace HoleriteAPI.Core.Domain.Ports
{
  public interface IHoleriteRepository
  {
    IReadOnlyList<FaixaINSS> CarregarFaixasINSS();

    TabelaIRRF CarregarTabelaIRRF();
  }
}
