using Dapper;
using System.Data;
using QCard.ManagementSystem.Domain.Modules.Monitoring.Model;

namespace QCard.ManagementSystem.Domain.Modules.Monitoring.Repository
{
    public partial interface IControlBatchProcessRepository : IDapperRepository<ControlBatchProcess>
    {
        int Update(string function, string status);
    }
    public partial class ControlBatchProcessRepository : DapperRepository<ControlBatchProcess>, IControlBatchProcessRepository
    {
        public ControlBatchProcessRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }

        public int Update(string function, string status)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@Status", status);
            parameters.Add("@Function", function);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute("USP_UPDATE_CONTROL_BATCH_PROCESS", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
    }
}
