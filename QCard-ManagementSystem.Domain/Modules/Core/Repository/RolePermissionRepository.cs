using Dapper;
using System.Data;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IRolePermissionRepository : IDapperRepository<RolePermissionView>
    {
    }

    public partial class RolePermissionRepository : DapperRepository<RolePermissionView>, IRolePermissionRepository
    {
        public RolePermissionRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }
    }
}

