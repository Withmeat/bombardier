using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bombardier_wpf.ViewModel
{
    public class bombardFields : ViewModelBase
    {
        
        
        
        public int X { get; set; }
        public int Y { get; set; }



        private int _data0;

        public int Data0
        {
            get { return _data0; }
            set
            {
                _data0 = value;
                OnPropertyChanged();
            }
        }
    }
}
