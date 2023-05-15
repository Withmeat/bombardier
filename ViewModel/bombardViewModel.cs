using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using bombardier_wpf.Model;
using bombardier_wpf.Persistence;

namespace bombardier_wpf.ViewModel
{
    public class bombardViewModel : ViewModelBase
    {

        #region Properties
        private bombardModel _model; // modell
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public ObservableCollection<bombardFields> Fields { get; set; }
        public String GameTime { get { return TimeSpan.FromSeconds(_model.GameTime).ToString("g"); } }
        public int Points { get => _model.Points; }
    

        public int Size
        {
            get { return _model.GameMap.size; }

        }


        public Boolean IsGameEasy
        {
           get { return _model.Size == size.Small; }
           set
            {
                if (_model.Size == size.Small)
                    return;

                _model.Size = size.Small;
              
                OnPropertyChanged(nameof(IsGameEasy));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameHard));
            }
        }
        public Boolean IsGameMedium
        {
            get  {return _model.Size == size.Medium; }
            set
            {
                if (_model.Size == size.Medium)
                    return;

                _model.Size = size.Medium;
             
                OnPropertyChanged(nameof(IsGameEasy));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameHard));
            }
        }
        public Boolean IsGameHard
        {
            get { return _model.Size == size.Big; }
            set
            {
                if (_model.Size == size.Big)
                    return;


                _model.Size = size.Big;
               
                OnPropertyChanged(nameof(IsGameEasy));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameHard));
            }
        }

        #endregion


        #region Events

        public event EventHandler? NewGame;    
        public event EventHandler? LoadGame;   
        public event EventHandler? SaveGame;
        public event EventHandler? ExitGame;
        #endregion



        #region Constructor
        
        public bombardViewModel(bombardModel model)
        {
            _model = model;
            _model.GameAdvanced += new EventHandler<bombardEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<bombardEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<bombardEventArgs>(Model_GameCreated);
            _model.NewPoint += new EventHandler<EventArgs>(Model_NewPoint);


            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            Fields = new ObservableCollection<bombardFields>();
            NewTable();

          
        }


        private void NewTable()
        {
            Fields.Clear();
            for (Int32 i = 0; i < _model.GameMap.Size; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model.GameMap.Size; j++)
                {
                    int value = -1;
                    if (_model.GameMap[i, j].Contains(Player.Fal))
                    {
                        value = 1;
                    }
                    else if (_model.GameMap[i, j].Contains(Player.Bombázó))
                    {
                        value = 2;
                    }
                    else if (_model.GameMap[i, j].Contains(Player.Ellenség))
                    {
                        value = 3;
                    }
                    else if (_model.GameMap[i, j].Contains(Player.Bomba))
                    {
                        value = 4;
                    }
                    Fields.Add(new bombardFields
                    {

                        X = i,
                        Y = j,
                        Data0 = 1

                    });
                }
            }
            RefreshTable();
        }

        #endregion

        public void RefreshTable()
        {
           
            foreach (bombardFields field in Fields) // inicializálni kell a mezőket is
            {

                int value = -1;
                if (_model.GameMap[field.X, field.Y].Contains(Player.Fal))
                {
                    value = 1;
                }
                else if (_model.GameMap[field.X, field.Y].Contains(Player.Bombázó))
                {
                    value = 2;
                }
                else if (_model.GameMap[field.X, field.Y].Contains(Player.Ellenség))
                {
                    value = 3;
                }
                else if (_model.GameMap[field.X, field.Y].Contains(Player.Bomba))
                {
                    value = 4;
                }
                field.Data0= value ;
            }
            OnPropertyChanged(nameof(GameTime));
            
        }

            #region Private methods refresh/stepgame needed
            #endregion

            #region Game event handlers not yet finished
        private void Model_GameCreated(object? sender, bombardEventArgs e)
        {
            NewTable();
         
        }

        private void Model_GameAdvanced(object? sender, bombardEventArgs e)
        {
            RefreshTable();
            OnPropertyChanged(nameof(GameTime));
        }
        private void Model_GameOver(object? sender, bombardEventArgs e)
        {
              RefreshTable();
        }
        private void Model_NewPoint(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Points));
        }
        #endregion
        #region Event methods
        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(Size));
            NewTable();
        }

       



        /// <summary>
        /// Játék betöltése eseménykiváltása.
        /// </summary>
        private void OnLoadGame()
        {
          
            LoadGame?.Invoke(this, EventArgs.Empty);
           
            OnPropertyChanged(nameof(IsGameEasy));
            OnPropertyChanged(nameof(IsGameMedium));
            OnPropertyChanged(nameof(IsGameHard));
          
            OnPropertyChanged(nameof(Size));
          

        }

        /// <summary>
        /// Játék mentése eseménykiváltása.
        /// </summary>
        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játékból való kilépés eseménykiváltása.
        /// </summary>
        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }
        #endregion

    }
}
