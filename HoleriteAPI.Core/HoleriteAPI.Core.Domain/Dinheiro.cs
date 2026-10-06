namespace HoleriteAPI.Core.Domain
{
  public static class Dinheiro
  {
    public static decimal Arredondar(decimal valor) =>
      decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
  }
}
