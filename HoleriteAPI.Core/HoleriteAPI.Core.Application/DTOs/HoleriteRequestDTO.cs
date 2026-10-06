using System.ComponentModel.DataAnnotations;

namespace HoleriteAPI.Core.Application.DTOs
{
  public abstract class HoleriteRequestDTO
  {
    private const string FormatoHoras = @"^\d{1,3}:[0-5]\d$";
    private const string MensagemHoras = "Use o formato HH:mm (ex.: 02:30).";

    [Range(0.01, 1_000_000, ErrorMessage = "SalarioBruto deve ser maior que zero.")]
    public decimal SalarioBruto { get; set; }

    [RegularExpression(FormatoHoras, ErrorMessage = MensagemHoras)]
    public string? HorasNoturnas { get; set; }

    [RegularExpression(FormatoHoras, ErrorMessage = MensagemHoras)]
    public string? HorasExtras75 { get; set; }

    [RegularExpression(FormatoHoras, ErrorMessage = MensagemHoras)]
    public string? HorasExtras100 { get; set; }

    [Range(1, 31, ErrorMessage = "DiasUteis deve estar entre 1 e 31.")]
    public int DiasUteis { get; set; }

    [Range(0, 31, ErrorMessage = "DomingosFeriados deve estar entre 0 e 31.")]
    public int DomingosFeriados { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "PlanoMedico não pode ser negativo.")]
    public decimal PlanoMedico { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "OutrosDescontos não pode ser negativo.")]
    public decimal OutrosDescontos { get; set; }
  }
}
