using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using TipCalculator.Core.Services;
using TipCalculator.Core.Services.Interfaces;
using TipCalculator.Core.ViewModels;

namespace TipCalculator.Core
{
  public class App : MvxApplication
  {
    public App()
    {
      Mvx.RegisterType<ICalculation, Calculation>(); //rejestrujemy typ odpowiadający za serwis -> gdy korzystamy z interfejsu ICalculation, to wykorzystywana będzie jego implementacja zawarta w klasie Calculation
      Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<TipViewModel>()); //rejestrujemy singleton odpowiadający za start aplikacji
    }
  }
}
