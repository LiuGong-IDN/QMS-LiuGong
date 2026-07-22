using Dapper;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Validator;
using System.Text.Json;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class RoleService : DbService
    {
        public RoleService(IDbHelper db) : base(db)
        {
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.RoleRepository.GetDataSourceResult(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public DataSourceResult GetRolePermissionList(string roleName, string functionName)
        {
            string methodName = "GetRolePermissionList";
            var result = new DataSourceResult();
            try
            {
                var rolePermission = Db.RoleRepository.GetRolePermissionList(roleName, functionName);
                result.Data = rolePermission;
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public BasicResponse Save(Role role)
        {
            string methodName = "Save";
            BasicResponse message = new();
            string passwordHash = string.Empty;
            try
            {
                //validator
                RoleValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(role);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                var result = Db.RoleRepository.Save(role, SessionManager.Id);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Update Data");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }

        public BasicResponse Update(List<RolePermissionView> rolePermission)
        {
            string methodName = "Save";
            BasicResponse message = new BasicResponse();
            try
            {
                var result = Db.RoleRepository.Update(rolePermission, SessionManager.Current);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Update Data");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }
    }
}
