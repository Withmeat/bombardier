using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bombardier_wpf.Model
{
    public class bombardEventArgs : EventArgs
    {

        private int _gameTime;
        private Boolean _isWon;


        public int GameTime { get { return _gameTime; } }
        public Boolean IsWon { get { return _isWon; } }


        public bombardEventArgs(Boolean isWon, int gameTime)
        {
            _isWon = isWon;
            _gameTime = gameTime;

        }
    }
}
