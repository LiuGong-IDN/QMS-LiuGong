using QCard.ManagementSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain
{
    public class DbServiceBase
    {
        private readonly IDbHelper _db;

        public DbServiceBase(IDbHelper db)
        {
            _db = db;
        }

        protected IDbHelper Db
        {
            get
            {
                return _db;
            }
        }

    }
}
