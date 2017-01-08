# Zadanie laboratoryjne - Wytw. oprogram. w środow. NET	
Poniższe zadanie ma przybliżyć tworzenie aplikacji w oparciu o wzorzec Model-View-ViewModel.

## Cel zadania
Poniższe zadanie ma za cel stworzenie prostej aplikacji wyliczającej napiwek dla kelnera na podstawie całkowitej kwoty zamówienia oraz "procentu" wartości zamówienia, który zdecydowaliśmy się przeznaczyć na napiwek.

## Ready, steady, go!
### Tworzymy pustą solucję:
W środowisku Visual Studio 2015 tworzymy nową pustą solucję: 
  * Przy tworzeniu nowego projektu wybieramy z drzewa "Templates" - Other Project Types => Visual Studio Solutions
  * Wybieramy "Blank solution" i nazywamy solucję: `TipCalculator`
  ![Screenshot1](http://i.imgur.com/ulm16rl.png)
  
### Portable Class Library
#### Tworzymy projekt
Po utworzeniu solucji, możemy dodać do niej pierwszy projekt.

Będzie to projekt typu `Portable Class Library`. 

Nazywamy go `TipCalculator.Core`. Tutaj będziemy dodawać za chwilę całą logikę aplikacji.
![Imgur](http://i.imgur.com/3udk1Ho.png)

Po dodaniu projektu, Visual Studio zapyta nas jeszcze o "targety" naszej biblioteki - w tym miejscu wystarczy, że wybierzemy opcje:
  * .NET Framework 4.5.1
  * Windows 8.1
  
![Imgur](http://i.imgur.com/6b7Em8W.png)

#### Dodajemy bibliotekę `MvvmCross`
Po utworzeniu projektu, Twoja solucja powinna wyglądać mniej więcej tak:
![Imgur](http://i.imgur.com/eOs2MAM.png)

Po utworzeniu projektu czas dodać bibliotekę `MvvmCross`, która jest dobrym wsparciem przy tworzeniu aplikacji w oparciu o wzorzec MVVM.
  * W tym celu wybieramy z menu Tools => NuGet Package Manager => `Package Manager Console`
  * Teraz powinno się pokazać okno w dolnej części Visual Studio, w które wpisujemy polecenie `Install-Package MvvmCross.Core` i potwierdzamy enterem:
![Imgur](http://i.imgur.com/IS3lNSY.png)

#### Tworzymy serwis wyliczający napiwek

 1. Usuń z projektu plik `Class1.cs` - nie będzie on potrzebny
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
 5. W folderze `Services` utwórz implementację tego interfejsu nazywając klasę `Calculation`
 
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
 6. To wszystko! Właśnie utworzyliśmy logikę biznesową naszej aplikacji. :)
 
#### Tworzymy nasz ViewModel

Interfejs naszej aplikacji powinien:

 * używać:
   * naszego serwisu, który będzie wyliczał wartość napiwka
 * posiadać możliwość wprowadzania:
   * wartości rachunku
   * procentu wartości rachunku, jakim chcemy nagrodzić kelnera
 * posiadać możliwość wyświetlania:
   * wartości napiwku

Aby umożliwić reprezentację interfejsu użytkownika musimy zbudować `model` go opisujący - czyli `ViewModel`

W `MvvmCross`, wszystkie `ViewModel-e` powinny dziedziczyć po klasie `MvxViewModel`.

Utwórz teraz nowy folder w projekcie o nazwie `ViewModels` i dodaj klasę `TipViewModel`:
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
        //TipViewModel jest tworzony razem z serwisem wyliczającym napiwek
        _calculation = calculation;
      }

      public override void Start()
      {
        //ustawiamy wstępne wartości
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
          RaisePropertyChanged(() => SubTotal);    //powiadamiamy klasę bazową MvxViewModel o tym że model się zmienił
          Recalcuate(); //przeliczamy wartość napiwku
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

Teraz pozostało nam stworzyć w głównym folderze projektu klasę `App`, w której zarejestrujemy nasz serwis oraz umożliwimy start aplikacji:

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
        //rejestrujemy typ odpowiadający za serwis -> gdy korzystamy z interfejsu ICalculation, to wykorzystywana będzie jego implementacja zawarta w klasie Calculation
        Mvx.RegisterType<ICalculation, Calculation>(); 
        //rejestrujemy singleton odpowiadający za start aplikacji
        Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<TipViewModel>()); 
    }
}
```

#### Projekt PCL gotowy :)

### Universal Windows Platform

#### Tworzymy projekt
Dodaj nowy projekt do solucji - `Blank App (Universal Windows)` o nazwie `TipCalculator.UWP`

![Imgur](http://i.imgur.com/MOxtNE8.png)

Po dodaniu projektu, struktura solucji powinna wyglądać mniej więcej tak:

![Imgur](http://i.imgur.com/XLuYjzQ.png)

#### Dodajemy MvvmCross

1. Usuwamy plik MainPage.xaml - nikomu on nie będzie potrzebny :)

2. Podobnie jak w poprzednim projekcie dodajemy bibliotekę `MvvmCross` poleceniem `Install-Package MvvmCross.Core` w `Package Manager Console`
![Imgur](http://i.imgur.com/1MmoC5G.png)

#### Dodajemy referencję do projektu TipCalculator.Core

1. Wybieramy prawym przyciskiem myszy menu kontekstowe pola `References` projektu `TipCalculator.UWP` i wybieramy `Add Reference...`
   
   ![Imgur](http://i.imgur.com/cQjtBd5.png)

2. Wyświetli się Reference Manager i w nim zaznaczymy projekt `TipCalculator.Core`

   ![Imgur](http://i.imgur.com/gOEeCFr.png)

#### Konfiguracja aplikacji UWP

1. Utwórz w głównym folderze projektu nową klasę `Setup` dziedziczącą po `MvxWindowsSetup`

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

2. W pliku `App.xaml.cs` zmieniamy następujące linijki w metodzie `OnLaunched`:
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
2. Dodaj w tym folderze nową stronę typu `Blank Page` i nazwij ją `TipView.xaml`
3. W folderze `Views` powinny zostać utworzone dwa pliki
 * TipView.xaml
 * TipView.xaml.cs
4. Przejdź do pliku TipView.xaml.cs i zmień klasę po której dziedziczy `TipView` z `Page` na `MvxWindowsPage`.
5. Twój plik TipView.xaml.cs powinien wyglądać tak:
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
6. Przejdź do pliku TipView.xaml i zmień jego zawartość na:
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
