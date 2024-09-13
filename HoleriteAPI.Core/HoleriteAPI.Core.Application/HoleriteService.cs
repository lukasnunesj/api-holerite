using HoleriteAPI.Core.Application.DTOs;
using HoleriteAPI.Core.Application.Interfaces;
using HoleriteAPI.Core.Domain;
using HoleriteAPI.Core.Domain.Ports;

namespace HoleriteAPI.Core.Application
{
  public class HoleriteService(IHoleriteRepository IHoleriteRepository) : IHoleriteService
  {
    private readonly IHoleriteRepository _IHoleriteRepository = IHoleriteRepository;

    public HoleriteResponseDTO CalculaTotais(HoleriteRequestDTO holeriteRequestDTO)
    {
      CargaHoraria folha = new()
      {
        Base = holeriteRequestDTO.SalarioBruto,
        TotalHorasNoturnas = TransformaHoras(holeriteRequestDTO.HorasNoturnas),
        ValorHoraTrabalho = holeriteRequestDTO.SalarioBruto / 200,
      };
      folha.ValorHoraNoturna = folha.ValorHoraTrabalho * 0.3;
      double totalAdicionalNoturno = folha.CalcularAdicionalNoturno();

      folha.TotalHorasExtras = TransformaHoras(holeriteRequestDTO.HorasExtras75);
      folha.Acrescimo = 1.75;
      double totalHorasExtras75 = folha.CalcularHorasExtras();

      folha.TotalHorasExtras = TransformaHoras(holeriteRequestDTO.HorasExtras100);
      folha.Acrescimo = 2;
      double totalHorasExtras100 = folha.CalcularHorasExtras();

      double totalHorasExtras = totalHorasExtras75 + totalHorasExtras100;

      double totalDSRNoturno = new CargaHoraria()
      {
        TotalHorasExtras = totalAdicionalNoturno,
        DiasUteis = holeriteRequestDTO.DiasUteis,
        DomingosFeriados = holeriteRequestDTO.DomingosFeriados
      }.CalcularDSRHorasExtras();

      double totalDSRHoraExtra = new CargaHoraria()
      {
        TotalHorasExtras = totalHorasExtras,
        DiasUteis = holeriteRequestDTO.DiasUteis,
        DomingosFeriados = holeriteRequestDTO.DomingosFeriados
      }.CalcularDSRHorasExtras();

      double BaseDeCalculo = holeriteRequestDTO.SalarioBruto + totalAdicionalNoturno + totalHorasExtras + totalDSRNoturno + totalDSRHoraExtra;
      double totalINSS = CalcularINSS(BaseDeCalculo);
      double totalIRRF = CalcularIRRF(BaseDeCalculo, totalINSS);

      double totalDebitos = BaseDeCalculo;
      double valorValeAdiantamento = holeriteRequestDTO.SalarioBruto * 0.40;
      double totalGeral = totalDebitos - valorValeAdiantamento;
      totalGeral -= holeriteRequestDTO.PlanoMedico;
      totalGeral -= holeriteRequestDTO.OutrosDescontos;
      totalGeral -= totalINSS;
      totalGeral -= totalIRRF;


      return new HoleriteResponseDTO(
        double.Round(holeriteRequestDTO.SalarioBruto, 2),
        double.Round(totalINSS, 2),
        double.Round(totalIRRF, 2),
        double.Round(totalAdicionalNoturno, 2),
        double.Round(totalHorasExtras75, 2),
        double.Round(totalHorasExtras100, 2),
        double.Round(totalDSRNoturno, 2),
        double.Round(totalDSRHoraExtra, 2),
        double.Round(totalDebitos, 2),
        double.Round(totalGeral, 2),
        double.Round(holeriteRequestDTO.PlanoMedico, 2),
        double.Round(holeriteRequestDTO.OutrosDescontos, 2),
        double.Round(valorValeAdiantamento, 2)
      );
    }

    private double CalcularINSS(double salario)
    {
      INSS inss = new(salario) { AliquotasINSS = PopularAliquotasINSS() };

      return inss.CalcularINSS();
    }

    private double CalcularIRRF(double salario, double parcelaINSS)
    {
      IRRF irrf = new(salario, parcelaINSS) { AliquotasIRRF = PopularAliquotasIRRF() };
      return irrf.CalcularIRRF();
    }

    private AliquotaIRRF[] PopularAliquotasIRRF()
    {
      return _IHoleriteRepository.CarregarAliquotasIRRF();
    }

    private AliquotaINSS[] PopularAliquotasINSS()
    {
      return _IHoleriteRepository.CarregarAliquotasINSS();
    }

    private static double TransformaHoras(string? horario)
    {
      var partes = horario?.Split(':') ?? ["00", "00"];
      if (partes.Length != 2 || !int.TryParse(partes[0], out int horas) || !int.TryParse(partes[1], out int minutos))
      {
        throw new ArgumentException("Formato de horário inválido. Use o formato HH:mm.", nameof(horario));
      }
      return horas + minutos / 60.0;
    }

  }
}
