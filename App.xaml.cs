using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using bombardier_wpf.Model;
using bombardier_wpf.Persistence;
using bombardier_wpf.ViewModel;
using bombardier_wpf.View;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Controls;

namespace bombardier_wpf
{


 
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _view = null!;
        private bombardModel _model = null!;
        private bombardViewModel _viewModel = null!;
        private DispatcherTimer _timer = null!;
        private Boolean time_stop;

        public App()
        {
            Startup += App_Startup;

        }



        private async void App_Startup(object sender, StartupEventArgs e)
        {

             _model = new bombardModel(new bombardDataAccess());
            _model.GameOver += new EventHandler<bombardEventArgs>(Model_GameOver);
           await  _model.NewGame(_model.Size);
         


         

            _viewModel = new bombardViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            _view = new MainWindow();
            _view.KeyDown += MainWindowKeyDown;
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            _view.Show();


            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            
            _timer.Start();

        }

     

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _model.AdvanceTime();
         
        }

        private void MainWindowKeyDown(object? sender, KeyEventArgs e)
        {

            if (e.Key == Key.P)
            {
                if (!time_stop)
                {
                    _timer.Stop();
                    time_stop = true;
                }
                else
                {
                    _timer.Start();
                    time_stop = false;
                }

            }

            if (!time_stop)
            {
                _model.KeyHandler(e.Key);
                _viewModel.RefreshTable();
            }
        }

        private void View_Closing(object? sender, CancelEventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Bombard", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _timer.Start();
            }
        }

        private void ViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
                saveFileDialog.Title = "Bombard pálya betöltése";
                saveFileDialog.Filter = "Bombard pálya|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        // játéktábla mentése
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (bombardDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("A fájl mentése sikertelen!", "Bombard", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start();
        }


        private async void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); // dialógusablak
                openFileDialog.Title = "Bombard pálya betöltése";
                openFileDialog.Filter = "Bombard pálya|*.txt";
                if (openFileDialog.ShowDialog() == true)
                {
                    // játék betöltése
                    await _model.LoadGameAsync(openFileDialog.FileName);

                    _timer.Start();
                }
            }
            catch (bombardDataException)
            {
                MessageBox.Show("A fájl betöltése sikertelen!", "Bombard", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start();
            time_stop = false;

        }

        private async void  ViewModel_NewGame(object? sender, System.EventArgs e)
        {
            time_stop = false;
            await _model.NewGame(_model.Size);
            _timer.Start();
        }

        private void Model_GameOver(object? sender, bombardEventArgs e)
        {
            _timer.Stop();

            if (e.IsWon) // győzelemtől függő üzenet megjelenítése
            {
                MessageBox.Show("Gratulálok, győztél!" + Environment.NewLine +
                                "Összesen " +
                                TimeSpan.FromSeconds(e.GameTime).ToString("g") + " ideig játszottál.",
                                "Bombard játék",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Sajnálom, vesztettél!",
                                "Bombard játék",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
        }


    }
}
