using Dapper;
using System.Data;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface ISettingRepository : IDapperRepository<Setting>
    {
    }

    public partial class SettingRepository : DapperRepository<Setting>, ISettingRepository
    {
        public SettingRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }
    }
}

