using QCard.ManagementSystem.Domain;

namespace QCard.ManagementSystem.Service
{
    public class DbService
    {
        private readonly IDbHelper _db;
        public DbService(IDbHelper db) { _db = db; }
        protected IDbHelper Db => _db;
    }
}
