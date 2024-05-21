
namespace HoleriteAPI.Core.Domain
{
  public class CargaHoraria
  {
    public double Base { get; set; }
    public double TotalHorasExtras { get; set; }
    public double TotalHorasNoturnas { get; set; }
    public double Acrescimo { get; set; }
    public int DiasUteis { get; set; }
    public int DomingosFeriados { get; set; }
    public double ValorHoraTrabalho { get; set; }
    public double ValorHoraNoturna { get; set; }

    public double CalcularHorasExtras()
    {
      return ValorHoraTrabalho * Acrescimo * TotalHorasExtras;
    }

    public double CalcularDSRHorasExtras()
    {
      return (TotalHorasExtras / DiasUteis) * DomingosFeriados;
    }

    public double CalcularAdicionalNoturno()
    {
      return TotalHorasNoturnas * ValorHoraNoturna;
    }
  }
}
