using HoleriteAPI.Core.Application.DTOs;
using HoleriteAPI.Core.Application.Interfaces;
using HoleriteAPI.Core.Domain;
using HoleriteAPI.Core.Domain.Ports;

namespace HoleriteAPI.Core.Application
{
  public class HoleriteService(IHoleriteRepository repository) : IHoleriteService
  {
    // Premissas do caso de uso original (jornada, adicionais e vale). Ver README.
    private const decimal HorasMensais = 200m;
    private const decimal PercentualAdicionalNoturno = 0.30m;
    private const decimal AcrescimoHoraExtra75 = 1.75m;
    private const decimal AcrescimoHoraExtra100 = 2m;
    private const decimal PercentualVale = 0.40m;

    public HoleriteResponseDTO CalculaTotais(HoleriteRequestDTO request)
    {
      decimal valorHora = CargaHoraria.ValorHora(request.SalarioBruto, HorasMensais);

      // Cada verba é arredondada em centavos, como num holerite, antes de somar.
      decimal adicionalNoturno = Dinheiro.Arredondar(
        CargaHoraria.AdicionalNoturno(valorHora, PercentualAdicionalNoturno, ParaHoras(request.HorasNoturnas)));
      decimal horasExtras75 = Dinheiro.Arredondar(
        CargaHoraria.HorasExtras(valorHora, AcrescimoHoraExtra75, ParaHoras(request.HorasExtras75)));
      decimal horasExtras100 = Dinheiro.Arredondar(
        CargaHoraria.HorasExtras(valorHora, AcrescimoHoraExtra100, ParaHoras(request.HorasExtras100)));
      decimal dsrNoturno = Dinheiro.Arredondar(
        CargaHoraria.DSR(adicionalNoturno, request.DiasUteis, request.DomingosFeriados));
      decimal dsrHorasExtras = Dinheiro.Arredondar(
        CargaHoraria.DSR(horasExtras75 + horasExtras100, request.DiasUteis, request.DomingosFeriados));

      decimal proventos = request.SalarioBruto + adicionalNoturno + horasExtras75 + horasExtras100 + dsrNoturno + dsrHorasExtras;

      decimal inss = INSS.Calcular(proventos, repository.CarregarFaixasINSS());
      decimal irrf = IRRF.Calcular(proventos, inss, repository.CarregarTabelaIRRF());

      decimal vale = Dinheiro.Arredondar(request.SalarioBruto * PercentualVale);
      decimal liquido = proventos - vale - request.PlanoMedico - request.OutrosDescontos - inss - irrf;

      return new HoleriteResponseDTO(
        SalarioBruto: Dinheiro.Arredondar(request.SalarioBruto),
        TotalIRRF: irrf,
        TotalINSS: inss,
        TotalAdicionalNoturno: adicionalNoturno,
        TotalHorasExtras75: horasExtras75,
        TotalHorasExtras100: horasExtras100,
        TotalDSRNoturno: dsrNoturno,
        TotalDSRHoraExtra: dsrHorasExtras,
        TotalDebitos: Dinheiro.Arredondar(proventos),
        TotalGeral: Dinheiro.Arredondar(liquido),
        PlanoMedico: Dinheiro.Arredondar(request.PlanoMedico),
        OutrosDescontos: Dinheiro.Arredondar(request.OutrosDescontos),
        ValorValeAdiantamento: vale
      );
    }

    private static decimal ParaHoras(string? horario)
    {
      if (string.IsNullOrEmpty(horario)) return 0;

      string[] partes = horario.Split(':');
      if (partes.Length != 2 || !int.TryParse(partes[0], out int horas) || !int.TryParse(partes[1], out int minutos))
      {
        throw new ArgumentException("Formato de horário inválido. Use o formato HH:mm.", nameof(horario));
      }
      return horas + minutos / 60m;
    }
  }
}
