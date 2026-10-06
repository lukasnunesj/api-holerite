namespace HoleriteAPI.Core.Application.DTOs
{
  /// <remarks>TotalDebitos é o total de proventos (base de cálculo do INSS e do IRRF); o nome é mantido por compatibilidade com o front.</remarks>
  public record HoleriteResponseDTO(
    decimal SalarioBruto,
    decimal TotalIRRF,
    decimal TotalINSS,
    decimal TotalAdicionalNoturno,
    decimal TotalHorasExtras75,
    decimal TotalHorasExtras100,
    decimal TotalDSRNoturno,
    decimal TotalDSRHoraExtra,
    decimal TotalDebitos,
    decimal TotalGeral,
    decimal PlanoMedico,
    decimal OutrosDescontos,
    decimal ValorValeAdiantamento
  );
}
