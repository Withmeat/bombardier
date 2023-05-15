using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bombardier_wpf.Persistence
{
    public interface IbombardDataAccess
    {
        Task<gameMap> LoadAsync(String path);
        Task SaveAsync(String path, gameMap table);

    }
}
