namespace HoleriteAPI.Core.Domain.Ports
{
  public interface IHoleriteRepository
  {
    AliquotaIRRF[] CarregarAliquotasIRRF();

    AliquotaINSS[] CarregarAliquotasINSS();
  }
}