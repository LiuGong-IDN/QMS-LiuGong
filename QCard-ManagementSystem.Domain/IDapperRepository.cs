using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain
{
    public interface IDapperRepository<T>
    {
        IEnumerable<T> Find(dynamic param);
        IEnumerable<T> Find(string query, dynamic param);
        IEnumerable<T> FindAll();
    }
}
