namespace HoleriteAPI.Core.Domain
{
  public class AliquotaINSS
  {
    public double De { get; set; }
    public double Ate { get; set; }
    public double Aliquota { get; set; }
  }
  public class INSS(double salario)
  {
    public double Salario { get; private set; } = salario;
    public AliquotaINSS[] AliquotasINSS { get; set; }

    public double CalcularINSS()
    {
      double totalINSS = 0;

      AliquotaINSS ultimaFaixa = AliquotasINSS.Last();

      Salario = Math.Min(Salario, ultimaFaixa.Ate);

      for (int i = 0; i < AliquotasINSS.Length; i++)
      {
        AliquotaINSS faixa = AliquotasINSS[i];

        double valorFaixa = Math.Min(Salario, faixa.Ate) - (i == 0 ? 0 : AliquotasINSS[i - 1].Ate);
        totalINSS += valorFaixa * faixa.Aliquota;

        if (Salario <= faixa.Ate)
        {
          break;
        }
      }

      return totalINSS;
    }
  }
}