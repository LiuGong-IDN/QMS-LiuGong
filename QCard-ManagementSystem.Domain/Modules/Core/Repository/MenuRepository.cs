using Dapper;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using System.Data;
using System.Drawing.Printing;
using static QCard.ManagementSystem.Domain.Constants.AppConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IMenuRepository : IDapperRepository<Menu>
    {
        List<Menu> GetMenuList();
        int Save(Menu menu, string[] columns);
        int Update(Menu menu, string[] columns);
        int Delete(int id);
    }

    public partial class MenuRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<Menu>(connection, transaction), IMenuRepository
    {
        public List<Menu> GetMenuList()
        {
            var result = Connection.Query<Menu>("USP_GET_MENU_LIST", transaction: null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure).ToList();
            return result;
        }

        public int Save(Menu menu, string[] columns)
        {
            var MenuObj = (object)Mapping(menu);
            return Connection.Execute(DynamicQuery.GetInsertQuery("TB_M_MENU", MenuObj, "ID", columns), MenuObj, Transaction);
        }

        public int Update(Menu menu, string[] columns)
        {
            var isValid = false;
            var parameters = new DynamicParameters();
            parameters.Add("@MenuId", menu.ID);
            parameters.Add("@Title", menu.TITLE);
            parameters.Add("@ParentId", menu.PARENT_ID);
            parameters.Add("@OrderIndex", menu.ORDER_INDEX);
            parameters.Add("@Url", menu.URL);
            parameters.Add("@Status", menu.STATUS);
            parameters.Add("@IconClass", menu.ICON_CLASS);
            parameters.Add("@Description", menu.DESCRIPTION);
            parameters.Add("@NewFunctionId", menu.FUNCTION_ID);
            parameters.Add("@By", menu.UPDATED_BY);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_MENU_UPDATE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int Delete(int id)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_MENU_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
    }
}

