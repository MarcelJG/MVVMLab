namespace TipCalculator.Core.Services.Interfaces
{
  public interface ICalculation
  {
    double TipAmount(double subTotal, int percent);
  }
}