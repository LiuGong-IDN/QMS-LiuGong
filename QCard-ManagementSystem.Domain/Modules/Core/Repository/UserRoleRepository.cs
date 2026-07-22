using Dapper;
using System.Data;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IUserRoleRepository : IDapperRepository<UserRoleView>
    {
    }

    public partial class UserRoleRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<UserRoleView>(connection, transaction), IUserRoleRepository
    {
    }
}

