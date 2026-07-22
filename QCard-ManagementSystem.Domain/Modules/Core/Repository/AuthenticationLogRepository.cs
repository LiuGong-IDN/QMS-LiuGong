using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IAuthenticationLogRepository : IDapperRepository<AuthenticationLog>
    {
        int Create(AuthenticationLog item, string[] columns);
    }

    public partial class AuthenticationLogRepository : DapperRepository<AuthenticationLog>, IAuthenticationLogRepository
    {
        public AuthenticationLogRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }

        public int Create(AuthenticationLog item, string[] columns)
        {
            var parameters = (object)Mapping(item);
            return Connection.Execute(DynamicQuery.GetInsertQuery("TB_L_AUTHENTICATION_LOG", parameters, "Id", columns), parameters, Transaction);
        }
    }
}
