# Zadanie laboratoryjne - Wytw. oprogram. w �rodow. NET	
Poni�sze zadanie ma przybli�y� tworzenie aplikacji w oparciu o wzorzec Model-View-ViewModel.

## Cel zadania
Poni�sze zadanie ma za cel stworzenie prostej aplikacji wyliczaj�cej napiwek dla kelnera na podstawie ca�kowitej kwoty zam�wienia oraz "procentu" warto�ci zam�wienia, kt�ry zdecydowali�my si� przeznaczy� na napiwek.

## Ready, steady, go!
### Tworzymy pust� solucj�:
W �rodowisku Visual Studio 2015 tworzymy now� pust� solucj�: 
  * Przy tworzeniu nowego projektu wybieramy z drzewa "Templates" - Other Project Types => Visual Studio Solutions
  * Wybieramy "Blank solution" i nazywamy solucj�: `TipCalculator`
  ![Screenshot1](http://i.imgur.com/ulm16rl.png)
  
### Portable Class Library
#### Tworzymy projekt
Po utworzeniu solucji, mo�emy doda� do niej pierwszy projekt.

B�dzie to projekt typu `Portable Class Library`. 

Nazywamy go `TipCalculator.Core`. Tutaj b�dziemy dodawa� za chwil� ca�� logik� aplikacji.
![Imgur](http://i.imgur.com/3udk1Ho.png)

Po dodaniu projektu, Visual Studio zapyta nas jeszcze o "targety" naszej biblioteki - w tym miejscu wystarczy, �e wybierzemy opcje:
  * .NET Framework 4.5.1
  * Windows 8.1
  
![Imgur](http://i.imgur.com/6b7Em8W.png)

#### Dodajemy bibliotek� `MvvmCross`
Po utworzeniu projektu, Twoja solucja powinna wygl�da� mniej wi�cej tak:
![Imgur](http://i.imgur.com/eOs2MAM.png)

Po utworzeniu projektu czas doda� bibliotek� `MvvmCross`, kt�ra jest dobrym wsparciem przy tworzeniu aplikacji w oparciu o wzorzec MVVM.
  * W tym celu wybieramy z menu Tools => NuGet Package Manager => `Package Manager Console`
  * Teraz powinno si� pokaza� okno w dolnej cz�ci Visual Studio, w kt�re wpisujemy polecenie `Install-Package MvvmCross.Core` i potwierdzamy enterem:
![Imgur](http://i.imgur.com/IS3lNSY.png)

#### Tworzymy serwis wyliczaj�cy napiwek

 1. Usu� z projektu plik `Class1.cs` - nie b�dzie on potrzebny
 2. Utw�rz nowy folder w projekcie o nazwie `Services`
 3. W folderze `Services` utw�rz nowy folder o nazwie `Interfaces`
 4. W folderze `Interfaces` utw�rz nowy interfejs o nazwie `ICalculation`
 
    ```c#
    namespace TipCalculator.Core.Services.Interfaces
    {
        public interface ICalculation
        {
             double TipAmount(double subTotal, int percent);
        }
    }
    ```
 5. W folderze `Services` utw�rz implementacj� tego interfejsu nazywaj�c klas� `Calculation`
 
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
 6. To wszystko! W�a�nie utworzyli�my logik� biznesow� naszej aplikacji. :)
 
#### Tworzymy nasz ViewModel

Interfejs naszej aplikacji powinien:

 * u�ywa�:
   * naszego serwisu, kt�ry b�dzie wylicza� warto�� napiwka
 * posiada� mo�liwo�� wprowadzania:
   * warto�ci rachunku
   * procentu warto�ci rachunku, jakim chcemy nagrodzi� kelnera
 * posiada� mo�liwo�� wy�wietlania:
   * warto�ci napiwku

Aby umo�liwi� reprezentacj� interfejsu u�ytkownika musimy zbudowa� `model` go opisuj�cy - czyli `ViewModel`

W `MvvmCross`, wszystkie `ViewModel-e` powinny dziedziczy� po klasie `MvxViewModel`.

Utw�rz teraz nowy folder w projekcie o nazwie `ViewModels` i dodaj klas� `TipViewModel`:
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
        //TipViewModel jest tworzony razem z serwisem wyliczaj�cym napiwek
        _calculation = calculation;
      }

      public override void Start()
      {
        //ustawiamy wst�pne warto�ci
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
          RaisePropertyChanged(() => SubTotal);    //powiadamiamy klas� bazow� MvxViewModel o tym �e model si� zmieni�
          Recalcuate(); //przeliczamy warto�� napiwku
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

Teraz pozosta�o nam stworzy� w g��wnym folderze projektu klas� `App`, w kt�rej zarejestrujemy nasz serwis oraz umo�liwimy start aplikacji:

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
        //rejestrujemy typ odpowiadaj�cy za serwis -> gdy korzystamy z interfejsu ICalculation, to wykorzystywana b�dzie jego implementacja zawarta w klasie Calculation
        Mvx.RegisterType<ICalculation, Calculation>(); 
        //rejestrujemy singleton odpowiadaj�cy za start aplikacji
        Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<TipViewModel>()); 
    }
}
```

#### Projekt PCL gotowy :)

### Universal Windows Platform

#### Tworzymy projekt
Dodaj nowy projekt do solucji - `Blank App (Universal Windows)` o nazwie `TipCalculator.UWP`
![Imgur](http://i.imgur.com/MOxtNE8.png)

Po dodaniu projektu, struktura solucji powinna wygl�da� mniej wi�cej tak:
![Imgur](http://i.imgur.com/XLuYjzQ.png)

#### Dodajemy MvvmCross

1. Usuwamy plik MainPage.xaml - nikomu on nie b�dzie potrzebny :)

2. Podobnie jak w poprzednim projekcie dodajemy bibliotek� `MvvmCross` poleceniem `Install-Package MvvmCross.Core` w `Package Manager Console`
![Imgur](http://i.imgur.com/1MmoC5G.png)

#### Dodajemy referencj� do projektu TipCalculator.Core

1. Wybieramy prawym przyciskiem myszy menu kontekstowe pola `References` projektu `TipCalculator.UWP` i wybieramy `Add Reference...`
   
   ![Imgur](http://i.imgur.com/cQjtBd5.png)

2. Wy�wietli si� Reference Manager i w nim zaznaczymy projekt `TipCalculator.Core`

   ![Imgur](http://i.imgur.com/gOEeCFr.png)

#### Konfiguracja aplikacji UWP

1. Utw�rz w g��wnym folderze projektu now� klas� `Setup` dziedzicz�c� po `MvxWindowsSetup`

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

2. W pliku `App.xaml.cs` zmieniamy nast�puj�ce linijki w metodzie `OnLaunched`:
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

1. Utw�rz w projekcie UWP folder `Views`
2. Dodaj w tym folderze now� stron� typu `Blank Page` i nazwij j� `TipView.xaml`
3. W folderze `Views` powinny zosta� utworzone dwa pliki
 * TipView.xaml
 * TipView.xaml.cs
4. Przejd� do pliku TipView.xaml.cs i zmie� klas� po kt�rej dziedziczy `TipView` z `Page` na `MvxWindowsPage`.
5. Tw�j plik TipView.xaml.cs powinien wygl�da� tak:
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
6. Przejd� do pliku TipView.xaml i zmie� jego zawarto�� na:
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