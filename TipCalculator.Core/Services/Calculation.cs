using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipCalculator.Core.Services.Interfaces;

namespace TipCalculator.Core.Services
{
  public class Calculation : ICalculation
  {
    public double TipAmount(double subTotal, int percent)
    {
      return subTotal * ((double) percent / 100);
    }
  }
}
