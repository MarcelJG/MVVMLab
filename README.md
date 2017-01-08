# Zadanie laboratoryjne - Wytw. oprogram. w œrodow. NET	
Poni¿sze zadanie ma przybli¿yæ tworzenie aplikacji w oparciu o wzorzec Model-View-ViewModel.

## Cel zadania
Poni¿sze zadanie ma za cel stworzenie prostej aplikacji wyliczaj¹cej napiwek dla kelnera na podstawie ca³kowitej kwoty zamówienia oraz "procentu" wartoœci zamówienia, który zdecydowaliœmy siê przeznaczyæ na napiwek.

## Ready, steady, go!
### Tworzymy pust¹ solucjê:
W œrodowisku Visual Studio 2015 tworzymy now¹ pust¹ solucjê: 
  * Przy tworzeniu nowego projektu wybieramy z drzewa "Templates" - Other Project Types => Visual Studio Solutions
  * Wybieramy "Blank solution" i nazywamy solucjê: `TipCalculator`
  ![Screenshot1](http://i.imgur.com/ulm16rl.png)
  
### Portable Class Library
#### Tworzymy projekt
Po utworzeniu solucji, mo¿emy dodaæ do niej pierwszy projekt.

Bêdzie to projekt typu `Portable Class Library`. 

Nazywamy go `TipCalculator.Core`. Tutaj bêdziemy dodawaæ za chwilê ca³¹ logikê aplikacji.
![Imgur](http://i.imgur.com/3udk1Ho.png)

Po dodaniu projektu, Visual Studio zapyta nas jeszcze o "targety" naszej biblioteki - w tym miejscu wystarczy, ¿e wybierzemy opcje:
  * .NET Framework 4.5.1
  * Windows 8.1
  
![Imgur](http://i.imgur.com/6b7Em8W.png)

#### Dodajemy bibliotekê `MvvmCross`
Po utworzeniu projektu, Twoja solucja powinna wygl¹daæ mniej wiêcej tak:
![Imgur](http://i.imgur.com/eOs2MAM.png)

Po utworzeniu projektu czas dodaæ bibliotekê `MvvmCross`, która jest dobrym wsparciem przy tworzeniu aplikacji w oparciu o wzorzec MVVM.
  * W tym celu wybieramy z menu Tools => NuGet Package Manager => `Package Manager Console`
  * Teraz powinno siê pokazaæ okno w dolnej czêœci Visual Studio, w które wpisujemy polecenie `Install-Package MvvmCross.Core` i potwierdzamy enterem:
![Imgur](http://i.imgur.com/IS3lNSY.png)

#### Tworzymy serwis wyliczaj¹cy napiwek

 1. Usuñ z projektu plik `Class1.cs` - nie bêdzie on potrzebny
 2. Utwórz nowy folder w projekcie o nazwie `Services`
 3. W folderze `Services` utwórz nowy folder o nazwie `Interfaces`
 4. W folderze `Interfaces` utwórz nowy interfejs o nazwie `ICalculation`
 
    ```c#
    namespace TipCalculator.Core.Services.Interfaces
    {
        public interface ICalculation
        {
             double TipAmount(double subTotal, int percent);
        }
    }
    ```
 5. W folderze `Services` utwórz implementacjê tego interfejsu nazywaj¹c klasê `Calculation`
 
     ```c#
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
     ```
 6. To wszystko! W³aœnie utworzyliœmy logikê biznesow¹ naszej aplikacji. :)
 
#### Tworzymy nasz ViewModel

Interfejs naszej aplikacji powinien:

 * u¿ywaæ:
   * naszego serwisu, który bêdzie wylicza³ wartoœæ napiwka
 * posiadaæ mo¿liwoœæ wprowadzania:
   * wartoœci rachunku
   * procentu wartoœci rachunku, jakim chcemy nagrodziæ kelnera
 * posiadaæ mo¿liwoœæ wyœwietlania:
   * wartoœci napiwku

Aby umo¿liwiæ reprezentacjê interfejsu u¿ytkownika musimy zbudowaæ `model` go opisuj¹cy - czyli `ViewModel`

W `MvvmCross`, wszystkie `ViewModel-e` powinny dziedziczyæ po klasie `MvxViewModel`.

Utwórz teraz nowy folder w projekcie o nazwie `ViewModels` i dodaj klasê `TipViewModel`:
```c#
using MvvmCross.Core.ViewModels;
using TipCalculator.Core.Services.Interfaces;

namespace TipCalculator.Core.ViewModels
{
    public class TipViewModel : MvxViewModel
    {
      private readonly ICalculation _calculation;

      public TipViewModel(ICalculation calculation)
      {
        //TipViewModel jest tworzony razem z serwisem wyliczaj¹cym napiwek
        _calculation = calculation;
      }

      public override void Start()
      {
        //ustawiamy wstêpne wartoœci
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
          RaisePropertyChanged(() => SubTotal);    //powiadamiamy klasê bazow¹ MvxViewModel o tym ¿e model siê zmieni³
          Recalcuate(); //przeliczamy wartoœæ napiwku
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
```

#### Konfiguracja aplikacji

Teraz pozosta³o nam stworzyæ w g³ównym folderze projektu klasê `App`, w której zarejestrujemy nasz serwis oraz umo¿liwimy start aplikacji:

```c#
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using TipCalculator.Core.Services;
using TipCalculator.Core.Services.Interfaces;
using TipCalculator.Core.ViewModels;

namespace TipCalculator.Core
{
    public class App : MvxApplication
    {
        //rejestrujemy typ odpowiadaj¹cy za serwis -> gdy korzystamy z interfejsu ICalculation, to wykorzystywana bêdzie jego implementacja zawarta w klasie Calculation
        Mvx.RegisterType<ICalculation, Calculation>(); 
        //rejestrujemy singleton odpowiadaj¹cy za start aplikacji
        Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<TipViewModel>()); 
    }
}
```

#### Projekt PCL gotowy :)

### Universal Windows Platform

#### Tworzymy projekt
Dodaj nowy projekt do solucji - `Blank App (Universal Windows)` o nazwie `TipCalculator.UWP`
![Imgur](http://i.imgur.com/MOxtNE8.png)

Po dodaniu projektu, struktura solucji powinna wygl¹daæ mniej wiêcej tak:
![Imgur](http://i.imgur.com/XLuYjzQ.png)

#### Dodajemy MvvmCross

1. Usuwamy plik MainPage.xaml - nikomu on nie bêdzie potrzebny :)

2. Podobnie jak w poprzednim projekcie dodajemy bibliotekê `MvvmCross` poleceniem `Install-Package MvvmCross.Core` w `Package Manager Console`
![Imgur](http://i.imgur.com/1MmoC5G.png)

#### Dodajemy referencjê do projektu TipCalculator.Core

1. Wybieramy prawym przyciskiem myszy menu kontekstowe pola `References` projektu `TipCalculator.UWP` i wybieramy `Add Reference...`
   
   ![Imgur](http://i.imgur.com/cQjtBd5.png)

2. Wyœwietli siê Reference Manager i w nim zaznaczymy projekt `TipCalculator.Core`

   ![Imgur](http://i.imgur.com/gOEeCFr.png)

#### Konfiguracja aplikacji UWP

1. Utwórz w g³ównym folderze projektu now¹ klasê `Setup` dziedzicz¹c¹ po `MvxWindowsSetup`

   ```c#
   using Windows.UI.Xaml.Controls;
   using MvvmCross.Core.ViewModels;
   using MvvmCross.WindowsUWP.Platform;

   namespace TipCalculator.UWP
   {
     public class Setup : MvxWindowsSetup
     {
       public Setup(Frame rootFrame) : base(rootFrame)
       {
       }

       protected override IMvxApplication CreateApp()
       {
         return new Core.App();`:
         
       }
     }
   }
   ```

2. W pliku `App.xaml.cs` zmieniamy nastêpuj¹ce linijki w metodzie `OnLaunched`:
   ```c#
   // When the navigation stack isn't restored navigate to the first page,
   // configuring the new page by passing required information as a navigation
   // parameter
   rootFrame.Navigate(typeof(MainPage), e.Arguments);
   ```
   na:
   ```c#
   var setup = new Setup(rootFrame);
   setup.Initialize();

   var start = Mvx.Resolve<IMvxAppStart>();
   start.Start();
   ```

#### Tworzymy widok

1. Utwórz w projekcie UWP folder `Views`
2. Dodaj w tym folderze now¹ stronê typu `Blank Page` i nazwij j¹ `TipView.xaml`
3. W folderze `Views` powinny zostaæ utworzone dwa pliki
 * TipView.xaml
 * TipView.xaml.cs
4. PrzejdŸ do pliku TipView.xaml.cs i zmieñ klasê po której dziedziczy `TipView` z `Page` na `MvxWindowsPage`.
5. Twój plik TipView.xaml.cs powinien wygl¹daæ tak:
   ```c#
   using MvvmCross.WindowsUWP.Views;

   // The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

   namespace TipCalculator.UWP.Views
   {
     /// <summary>
     /// An empty page that can be used on its own or navigated to within a Frame.
     /// </summary>
     public sealed partial class TipView : MvxWindowsPage
     {
       public TipView()
       {
         this.InitializeComponent();
       }
     }
   }
   ```
6. PrzejdŸ do pliku TipView.xaml i zmieñ jego zawartoœæ na:
   ```xaml
   <views:MvxWindowsPage
    x:Class="TipCalculator.UWP.Views.TipView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:views="using:MvvmCross.WindowsUWP.Views"
    mc:Ignorable="d">

      <Grid Padding="20" Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
       <StackPanel Margin="12,0,12,0">
         <TextBlock Text="SubTotal" />
         <TextBox Text="{Binding SubTotal, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
         <TextBlock Text="Percent" />
         <Slider Value="{Binding Percent,Mode=TwoWay}" 
                   SmallChange="1" 
                   LargeChange="10" 
                   Minimum="0" 
                   Maximum="100" />
         <TextBlock Text="Tip" />
         <TextBlock Text="{Binding Tip}" />
       </StackPanel>
     </Grid>
   </views:MvxWindowsPage>
   ```
### To wszystko! Twoja aplikacja jest gotowa!
![Imgur](http://i.imgur.com/AArn1Ns.png)