using MvvmCross.Core.ViewModels;
using TipCalculator.Core.Services.Interfaces;

namespace TipCalculator.Core.ViewModels
{
  public class TipViewModel : MvxViewModel
  {
    private readonly ICalculation _calculation;

    public TipViewModel(ICalculation calculation)
    {
      _calculation = calculation;
    }

    public override void Start()
    {
      _subTotal = 100;
      _percent = 10;
      Recalcuate();
      base.Start();
    }

    private double _subTotal;

    public double SubTotal
    {
      get { return _subTotal; }
      set
      {
        _subTotal = value;
        RaisePropertyChanged(() => SubTotal);
        Recalcuate();
      }
    }

    private int _percent;

    public int Percent
    {
      get { return _percent; }
      set
      {
        _percent = value;
        RaisePropertyChanged(() => Percent);
        Recalcuate();
      }
    }

    private double _tip;

    public double Tip
    {
      get { return _tip; }
      set
      {
        _tip = value;
        RaisePropertyChanged(() => Tip);
      }
    }

    private void Recalcuate()
    {
      Tip = _calculation.TipAmount(SubTotal, Percent);
    }
  }
}