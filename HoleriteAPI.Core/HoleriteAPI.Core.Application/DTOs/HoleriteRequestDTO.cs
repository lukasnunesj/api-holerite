namespace HoleriteAPI.Core.Application.DTOs
{
  public abstract class HoleriteRequestDTO
  {
    public double SalarioBruto { get; set; }
    public string? HorasNoturnas { get; set; }
    public string? HorasExtras75 { get; set; }
    public string? HorasExtras100 { get; set; }
    public int DiasUteis { get; set; }
    public int DomingosFeriados { get; set; }
    public double PlanoMedico { get; set; }
    public double OutrosDescontos { get; set; }
  }
}
