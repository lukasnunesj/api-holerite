using HoleriteAPI.Core.Domain;
using HoleriteAPI.Core.Domain.Ports;

namespace HoleriteAPI.Data.Repositories
{
  public class HoleriteRepository : IHoleriteRepository
  {
    public AliquotaINSS[] CarregarAliquotasINSS()
    {
      return [
        new AliquotaINSS{ De= 0, Ate= 1412.00, Aliquota= 0.075 },
        new AliquotaINSS{ De= 1412.01, Ate= 2666.68, Aliquota= 0.09 },
        new AliquotaINSS{ De= 2666.69, Ate= 4000.03, Aliquota= 0.12 },
        new AliquotaINSS{ De= 4000.04, Ate= 7786.02, Aliquota= 0.14 },
      ];
    }

    public AliquotaIRRF[] CarregarAliquotasIRRF()
    {
      return
      [
        new AliquotaIRRF{ De = 0, Ate = 2259.20, Aliquota = 0, Deducao = 0 },
        new AliquotaIRRF{ De = 2259.21, Ate = 2826.65, Aliquota = 0.075, Deducao = 169.44 },
        new AliquotaIRRF{ De = 2826.65, Ate = 3751.06, Aliquota = 0.15, Deducao = 381.44 },
        new AliquotaIRRF{ De = 3751.06, Ate = 4664.68, Aliquota = 0.225, Deducao = 662.77 },
        new AliquotaIRRF{ De = 4664.69, Ate = 99999999999, Aliquota = 0.275, Deducao = 896.00 }
      ];
    }
  }
}