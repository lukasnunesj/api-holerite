namespace HoleriteAPI.Core.Application.DTOs
{
  public class HoleriteResponseDTO(double totalIRRF, double totalINSS, double totalAdicionalNoturno, double totalHorasExtras75, double totalHorasExtras100, double totalDSRNoturno, double totalDSRHoraExtra)
  {
    public double TotalIRRF { get; set; } = totalIRRF;
    public double TotalINSS { get; set; } = totalINSS;

    public double TotalAdicionalNoturno { get; set; } = totalAdicionalNoturno;

    public double TotalHorasExtras75 { get; set; } = totalHorasExtras75;

    public double TotalHorasExtras100 { get; set; } = totalHorasExtras100;

    public double TotalDSRNoturno { get; set; } = totalDSRNoturno;

    public double TotalDSRHoraExtra { get; set; } = totalDSRHoraExtra;

  }
}
