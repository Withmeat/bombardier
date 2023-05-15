using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using bombardier_wpf.Persistence;
using System.Windows.Controls;

namespace bombardier_wpf.Model
{

    public enum size { Small, Medium, Big };
    public class bombardModel
    {
       

            private int _gameTime;
            private IbombardDataAccess _dataAccess;
            private gameMap _gameMap;
            private size _size;
            private Boolean bombDropped ;

             private Int32 _points;

           





        public int GameTime { get { return _gameTime; } }
        public Int32 Points => _points;
        public gameMap GameMap { get { return _gameMap; } }

            public size Size { get { return _size; } set { _size = value; } }


            public event EventHandler<bombardEventArgs>? GameAdvanced;
            public event EventHandler<bombardEventArgs>? GameOver;
            public event EventHandler<bombardEventArgs>? GameCreated;
            public event EventHandler<EventArgs>? NewPoint;

        public bombardModel(IbombardDataAccess dataAccess)
            {

                _dataAccess = dataAccess;

                _gameMap = new gameMap();
                _size = size.Medium;
                


            }

            public Boolean isGameOver
            {
                get
                {
                    return (_gameMap.GetEnemies().Count == 0 || _gameMap.isLost);
                }
            }



            public async Task NewGame(size _size)
            {

            _points=0;
            switch (_size) // nehézségfüggő beállítása az időnek, illetve a generált mezőknek
                {

                    case size.Small:
                        _gameMap = new gameMap(20);
                        break;
                    case size.Medium:
                        _gameMap = new gameMap(25);
                        break;
                    case size.Big:
                        _gameMap = new gameMap(30);
                        break;
                }




                String path = "";

                switch (_size) // nehézségfüggő beállítása az időnek, illetve a generált mezőknek
                {
                    case size.Small:
                        path = "1.palya.txt";
                        break;
                    case size.Medium:
                        path = "2.palya.txt";
                        break;
                    case size.Big:
                        path = "3.palya.txt";
                        break;
                }

                

                await LoadGameAsync(path);
            OnNewPoints();


        }



            public async Task LoadGameAsync(String path)
            {
                if (_dataAccess == null)
                    throw new InvalidOperationException("Nem megfelelő txt");

                _gameMap = await _dataAccess.LoadAsync(path);
            switch (_gameMap.size) // nehézségfüggő beállítása az időnek, illetve a generált mezőknek
            {

                case 20:
                    _size = size.Small;
                    break;
                case 25:
                    _size = size.Medium;
                    break;
                case 30:
                    _size = size.Big;
                    break;
            }
            _gameTime = 0;

                OnGameCreated();
            }

            public async Task SaveGameAsync(String path)
            {
                if (_dataAccess == null)
                    throw new InvalidOperationException("Nem megfelelő txt");

                await _dataAccess.SaveAsync(path, _gameMap);
            }





            public void AdvanceTime()
            {
                    leptetes();
                    _gameTime++;

                OnGameAdvanced();

                if (_gameMap.lost)
                    OnGameOver(false);

                if (isGameOver)
                    return;

            }


            private void OnGameAdvanced()
            {
                GameAdvanced?.Invoke(this, new bombardEventArgs(false, _gameTime));
                if (_gameMap.isLost)
                    OnGameOver(false);
            }

            private void OnGameOver(Boolean isWon)
            {
                GameOver?.Invoke(this, new bombardEventArgs(isWon, _gameTime));
            }

             private void OnGameCreated()
             {
                    GameCreated?.Invoke(this, new bombardEventArgs(false, _gameTime));
             }

             private void OnNewPoints()
             {
             NewPoint?.Invoke(this, new EventArgs());
             }

        public void KeyHandler(Key e)
            {
                
                

                if (isGameOver)
                    return;

            int x = _gameMap.GetBomber().Item1;
                int y = _gameMap.GetBomber().Item2;

            if (x < _gameMap.Size && y < _gameMap.Size && x >= 0 && y >= 0)
                {
                    if (e == Key.W)
                    {
                        if (_gameMap.possibleStep(x, y).Contains((x - 1, y)))
                        {

                            _gameMap.MovePos(x, y, x - 1, y);

                            OnGameAdvanced();

                        }
                    }
                    else if (e== Key.A)
                    {
                        if (_gameMap.possibleStep(x, y).Contains((x, y - 1)))
                        {
                            _gameMap.MovePos(x, y, x, y - 1);
                            OnGameAdvanced();
                        }
                    }
                    else if (e == Key.S)
                    {
                        if (_gameMap.possibleStep(x, y).Contains((x + 1, y)))
                        {
                            _gameMap.MovePos(x, y, x + 1, y);
                            OnGameAdvanced();
                        }
                    }
                    else if (e == Key.D)
                    {
                        if (_gameMap.possibleStep(x, y).Contains((x, y + 1)))
                        {
                            _gameMap.MovePos(x, y, x, y + 1);
                            OnGameAdvanced();
                        }
                    }

                    else if (e == Key.Space)
                    {
                    if (!bombDropped)
                    {
                        for (int i = GameMap.GetBomber().Item1 - 3; i < GameMap.GetBomber().Item1 + 4; i++)
                        {
                            for (int j = GameMap.GetBomber().Item2 - 3; j < (GameMap.GetBomber().Item2 + 4); j++)
                            {
                                if (i >= 0 && i < GameMap.Size && j >= 0 && j < GameMap.Size)
                                {
                                    _gameMap.paintItBlack(i, j);
                                }
                            }
                        }
                        bombDropped = true;
                        delay();
                        if (_gameMap.won)
                            OnGameOver(true);
                        OnGameAdvanced();

                    }
                    }
                }





            }
        private async void delay()
        {
            await Task.Delay(5000);
            int temp = _gameMap.GetEnemies().Count;
            _gameMap.paintItWhite();
            int temp2 = _gameMap.GetEnemies().Count;
            if (temp > temp2)
            {
                _points++;
                OnNewPoints();
            }
                if (GameMap.GetEnemies().Count == 0)
                OnGameOver(true);
            bombDropped = false;
           
        }

        private void leptetes()
        {
            for (int i = 0; i < _gameMap.GetEnemies().Count; i++)
            {

                int x = _gameMap.GetEnemies()[i].Item1;
                int y = _gameMap.GetEnemies()[i].Item2;
                Boolean goodDirection = false;
                switch (_gameMap.EnemyPosandDir[i].Item2)
                {
                    case Direction.Up:
                        {

                            goodDirection = _gameMap.possibleStep(x, y).Contains((x - 1, y));
                            if (goodDirection)
                            {
                                _gameMap.MovePos(x, y, x - 1, y);
                                _gameMap.EnemyPosandDir[i] = new Tuple<(int, int), Direction>((x - 1, y), _gameMap.EnemyPosandDir[i].Item2);

                            }
                            else
                            {

                            }

                        }
                        break;
                    case Direction.Down:
                        {

                            goodDirection = _gameMap.possibleStep(x, y).Contains((x + 1, y));
                            if (goodDirection)
                            {
                                _gameMap.MovePos(x, y, x + 1, y);
                                _gameMap.EnemyPosandDir[i] = new Tuple<(int, int), Direction>((x + 1, y), _gameMap.EnemyPosandDir[i].Item2);

                            }

                        }
                        break;
                    case Direction.Left:
                        {

                            goodDirection = _gameMap.possibleStep(x, y).Contains((x, y - 1));
                            if (goodDirection)
                            {
                                _gameMap.MovePos(x, y, x, y - 1);
                                _gameMap.EnemyPosandDir[i] = new Tuple<(int, int), Direction>((x, y - 1), _gameMap.EnemyPosandDir[i].Item2);

                            }



                        }
                        break;
                    case Direction.Right:
                        {

                            goodDirection = _gameMap.possibleStep(x, y).Contains((x, y + 1));
                            if (goodDirection)
                            {
                                _gameMap.MovePos(x, y, x, y + 1);
                                _gameMap.EnemyPosandDir[i] = new Tuple<(int, int), Direction>((x, y + 1), _gameMap.EnemyPosandDir[i].Item2);

                            }

                        }
                        break;
                }
                if (!goodDirection)
                {
                    Random random = new Random();
                    Type type = typeof(Direction);
                    Array values = type.GetEnumValues();
                    int index = random.Next(values.Length);
                    Direction value = (Direction)values.GetValue(index);
                    _gameMap.EnemyPosandDir[i] = new Tuple<(int, int), Direction>((x, y), value);
                }



            }

        }

      


        }
    
}
