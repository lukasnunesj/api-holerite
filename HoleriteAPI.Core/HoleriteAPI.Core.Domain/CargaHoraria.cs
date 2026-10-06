namespace HoleriteAPI.Core.Domain
{
  public static class CargaHoraria
  {
    public static decimal ValorHora(decimal salario, decimal horasMensais) => salario / horasMensais;

    /// <summary>Horas extras são pagas por inteiro (hora x acréscimo), pois não estão no salário mensal.</summary>
    public static decimal HorasExtras(decimal valorHora, decimal acrescimo, decimal horas) =>
      valorHora * acrescimo * horas;

    /// <summary>As horas noturnas já estão no salário; paga-se só o adicional sobre elas.</summary>
    public static decimal AdicionalNoturno(decimal valorHora, decimal percentual, decimal horas) =>
      horas * valorHora * percentual;

    /// <summary>Descanso semanal remunerado sobre uma verba variável: (valor / dias úteis) x domingos e feriados.</summary>
    public static decimal DSR(decimal valor, int diasUteis, int domingosFeriados) =>
      valor / diasUteis * domingosFeriados;
  }
}
