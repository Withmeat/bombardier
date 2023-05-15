using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace bombardier_wpf.Persistence
{

    public enum Direction { Up, Down, Left, Right };
    public enum Player { Talaj, Fal, Bombázó, Ellenség, Bomba };
    public class gameMap
    {


        private List<Player>[,] values;
        public int size { get { return values.GetLength(0); } }
        public List<Tuple<(int, int), Direction>> EnemyPosandDir { get; } = new List<Tuple<(int, int), Direction>>();

        public gameMap() : this(25) { }
        public Boolean lost;
        public Boolean won;


        public gameMap(int size)
        {
            lost = false;
            won = false;

            values = new List<Player>[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    values[i, j] = new List<Player>();
                    values[i, j].Add(Player.Talaj);
                }


        }

        public int Size { get { return values.GetLength(0); } }
        public List<Player> this[int x, int y] { get { return GetValue(x, y); } set { values[x, y] = value; } }


        public List<Player> GetValue(int x, int y)
        {
            if (x < 0 || x > values.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The x coordinate is out of range!");
            if (y < 0 || y >= values.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The y coordinate is out of range!");

            return values[x, y];
        }

        public void SetValue(int x, int y, Player value)
        {


            if (x < 0 || x >= values.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= values.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");
            if (value != Player.Fal && value != Player.Bomba && value != Player.Bombázó && value != Player.Ellenség && value != Player.Talaj)
                throw new ArgumentOutOfRangeException("value", "The value is not listed.");
            if (possibleStep(x, y).Count == 0)
                return;
            values[x, y].Add(value);
        }


        public Boolean isLost
        {
            get
            {
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    for (int j = 0; j < values.GetLength(1); j++)
                    {
                        for (int k = 0; k < EnemyPosandDir.Count(); k++)
                        {
                            if (values[i, j].Contains(Player.Bombázó) && values[i, j].Contains(Player.Ellenség))
                                return true;
                        }
                    }
                }
                return false;
            }


        }


        #region GetItems
        public (int, int) GetBomber()
        {
            for (int i = 0; i < values.GetLength(0); i++)
                for (int j = 0; j < values.GetLength(1); j++)
                    if (values[i, j].Contains(Player.Bombázó))
                        return (i, j);
            return (-444, -444);
        }

        public void InitDirections()
        {
            EnemyPosandDir.Clear();
            var enemies = GetEnemies();
            Random random = new Random();
            for (int i = 0; i < enemies.Count(); i++)
            {
                //  Talaj, Fal, Ellenség, Bomba, Bombázó
                int x = enemies[i].Item1;
                int y = enemies[i].Item2;


                Type type = typeof(Direction);
                Array values = type.GetEnumValues();
                int index = random.Next(values.Length);
                Direction value = (Direction)values.GetValue(index);
                EnemyPosandDir.Add(new Tuple<(int, int), Direction>((x, y), value));

            }

        }


        public List<(int, int)> GetEnemies()
        {
            List<(int, int)> output = new List<(int, int)>();

            for (int i = 0; i < values.GetLength(0); i++)
                for (int j = 0; j < values.GetLength(1); j++)
                    if (values[i, j].Contains(Player.Ellenség))
                        output.Add((i, j));

            return output;

        }

        public List<(int, int)> GetBomb()
        {
            List<(int, int)> output = new List<(int, int)>();

            for (int i = 0; i < values.GetLength(0); i++)
                for (int j = 0; j < values.GetLength(1); j++)
                    if (values[i, j].Contains(Player.Bomba))
                        output.Add((i, j));

            return output;

        }

        public List<(int, int)> GetWalls()
        {
            List<(int, int)> output = new List<(int, int)>();

            for (int i = 0; i < values.GetLength(0); i++)
                for (int j = 0; j < values.GetLength(1); j++)
                    if (values[i, j].Contains(Player.Fal))
                        output.Add((i, j));

            return output;

        }

        #endregion

        public void MovePos(int fromX, int fromY, int toX, int toY)
        {



            if (values[fromX, fromY].Contains(Player.Ellenség))
            {
                if (values[toX, toY].Contains(Player.Bombázó))
                {
                    lost = true;
                }

                values[toX, toY].Add(Player.Ellenség);
                values[fromX, fromY].Remove(Player.Ellenség);
            }
            if (values[fromX, fromY].Contains(Player.Bombázó))
            {
                values[toX, toY].Add(Player.Bombázó);
                values[fromX, fromY].Remove(Player.Bombázó);
            }
        }

        public List<(int, int)> possibleStep(int x, int y)
        {
            List<(int, int)> output = new List<(int, int)>();

            if (x + 1 < Size && !(values[x + 1, y].Contains(Player.Fal)))  // le
                output.Add((x + 1, y));

            if (x - 1 >= 0 && !(values[x - 1, y].Contains(Player.Fal)))  // fel
                output.Add((x - 1, y));
            if (y - 1 >= 0 && !(values[x, y - 1].Contains(Player.Fal)))  // balra
                output.Add((x, y - 1));
            if (y + 1 < Size && !(values[x, y + 1].Contains(Player.Fal)))  // jobbra
                output.Add((x, y + 1));
            return output;
        }

        public void paintItBlack(int x, int y)
        {
            if (!(values[x, y].Contains(Player.Fal)))
            {
                values[x, y].Add(Player.Bomba);
                values[x, y].Remove(Player.Talaj);
            }
        }

        public void paintItWhite()
        {

            for (int i = 0; i < values.GetLength(0); i++)
                for (int j = 0; j < values.GetLength(1); j++)
                    if (values[i, j].Contains(Player.Bomba))
                    {
                        if (values[i, j].Contains(Player.Ellenség) && GetEnemies().Count == 1)
                        {
                            won = true;
                        }

                        values[i, j].Remove(Player.Bomba);
                        values[i, j].Remove(Player.Ellenség);

                        if (values[i, j].Contains(Player.Bombázó))
                        {
                            lost = true;
                            values[i, j].Remove(Player.Bombázó);
                        }

                        values[i, j].Add(Player.Talaj);

                    }
        }

    }
}
