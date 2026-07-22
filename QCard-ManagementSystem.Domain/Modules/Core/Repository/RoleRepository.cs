using Dapper;
using Kendo.Mvc.Infrastructure.Implementation;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IRoleRepository : IDapperRepository<Role>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null);
        List<RolePermissionView> GetRolePermissionList(string roleName, string functionName);
        public int Update(List<RolePermissionView> permission, string username);
        int Save(Role role, long by);
    }

    public partial class RoleRepository : DapperRepository<Role>, IRoleRepository
    {
        public RoleRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<Role>();

            return output;
        }
        public int Save(Role role, long by)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@RoleName", role.ROLE_NAME);
            parameters.Add("@Description", role.DESCRIPTION);
            parameters.Add("@By", by);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_ROLE_SAVE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }

        public List<RolePermissionView> GetRolePermissionList(string roleName, string functionName)
        {
            List<RolePermissionView> list = new List<RolePermissionView>();

            var parameters = new Dictionary<string, object>();
            parameters.Add("RoleName", roleName);
            parameters.Add("FunctionName", "%"+functionName+"%");

            list = Connection.Query<RolePermissionView>("USP_GET_ROLE_PERMISSION_LIST", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure).ToList();

            return list;
        }

        public int Update(List<RolePermissionView> permission, string username)
        {
            var isValid = false;
            var json = System.Text.Json.JsonSerializer.Serialize(permission.ToList());

            var parameters = new DynamicParameters();
            parameters.Add("@Data", json);
            parameters.Add("@Username", username);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_UPDATE_ROLE_PERMISSION", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
    }
}

