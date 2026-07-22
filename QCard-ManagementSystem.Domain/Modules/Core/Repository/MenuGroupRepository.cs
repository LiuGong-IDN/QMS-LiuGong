using System.Data;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IMenuGroupRepository : IDapperRepository<MenuGroup>
    {
    }

    public partial class MenuGroupRepository : DapperRepository<MenuGroup>, IMenuGroupRepository
    {
        public MenuGroupRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }
    }
}

