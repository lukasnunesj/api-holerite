namespace HoleriteAPI.Core.Domain
{
  public class AliquotaIRRF
  {
    public double De { get; set; }
    public double Ate { get; set; }
    public double Aliquota { get; set; }
    public double Deducao { get; set; }
  }

  public class IRRF(double salario, double parcelaINSS)
  {
    public AliquotaIRRF[] AliquotasIRRF { get; set; }

    public double Salario { get; private set; } = salario;

    public double ParcelaINSS { get; private set; } = parcelaINSS;

    public double CalcularIRRF()
    {
      double baseCalculo = Salario - ParcelaINSS;
      AliquotaIRRF? faixa = AliquotasIRRF.FirstOrDefault(x => baseCalculo >= x.De && baseCalculo <= x.Ate);
      double totalIRRF = (baseCalculo * faixa.Aliquota) - faixa.Deducao;
      return totalIRRF;
    }

  }
}
