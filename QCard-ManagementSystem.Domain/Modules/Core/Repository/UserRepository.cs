using Dapper;
using Kendo.Mvc.UI;
using Modules.MyMachines.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using System.Data;
using System.Text;
using System.Text.Json;
using CommandType = System.Data.CommandType;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IUserRepository : IDapperRepository<User>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null);
        List<UserView> ListUser(string type, string id, string phoneNumber, string email);
        int Save(UserRequest user, string passwordHash, string username);
        int Delete(string id);
        int ValidateUploadProcess(List<UserUpload> data, string username, out string tempCode, out string msgError);
        DataSourceResult GetUploadUserTemporary(DataSourceRequest request, object parameter = null);
        int SubmitFinalUpload(string tempCode, string username, out string msgError);
        int InsertPasswordResetToken(PasswordResetTokenRequest token);
        int ValidateAndUsePasswordResetToken(string tokenHash, string newPasswordHash);
        List<UserUploadResponse> GetUserUploadValidationByTempCode(string tempCode);

    }

    public partial class UserRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<User>(connection, transaction), IUserRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<UserView>();

            return output;
        }
        public List<UserView> ListUser(string type, string id, string phoneNumber, string email)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", type);
            GetListParams.Add("@Id", id);
            GetListParams.Add("@PhoneNumber", phoneNumber);
            GetListParams.Add("@Email", email);

            List<UserView> result = [.. Connection.Query<UserView>(
                "USP_GET_USER",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public int Save(UserRequest user, string passwordHash, string username)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", user.ID);
            parameters.Add("@FirstName", user.FIRST_NAME);
            parameters.Add("@LastName", user.LAST_NAME);
            parameters.Add("@Password", passwordHash);
            parameters.Add("@PhoneNumber", user.PHONE_NUMBER);
            parameters.Add("@Email", user.EMAIL);
            parameters.Add("@Company", user.COMPANY);
            parameters.Add("@UserRole", string.Join(',', user.ROLE));
            parameters.Add("@PDC", user.PDC);
            parameters.Add("@Qcard", user.QCARD);
            parameters.Add("@By", username);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_USER_SAVE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int Delete(string id)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_USER_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int ValidateUploadProcess(List<UserUpload> data, string username, out string tempCode, out string msgError)
        {
            var isValid = false;
            msgError = string.Empty;
            tempCode = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var json = JsonSerializer.Serialize(data);

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@Data", json);
            parameters.Add("@UserName", username);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
            parameters.Add("@MsgError", msgError, DbType.String, ParameterDirection.Output, 4000);

            Connection.Execute("[dbo].[USP_USER_UPLOAD]", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("@IsValid");
            msgError = parameters.Get<string>("@MsgError");

            return isValid ? 1 : 0;
        }
        public DataSourceResult GetUploadUserTemporary(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<UserUpload>();

            return output;
        }
        public int SubmitFinalUpload(string tempCode, string username, out string msgError)
        {
            var isValid = false;
            msgError = string.Empty;

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@UserName", username);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
            parameters.Add("@MsgError", msgError, DbType.String, ParameterDirection.Output, 4000);

            Connection.Execute("[dbo].[USP_USER_UPLOAD_SUBMIT]", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            msgError = parameters.Get<string>("MsgError");

            return isValid ? 1 : 0;
        }
        public int InsertPasswordResetToken(PasswordResetTokenRequest token)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", token.Email);
            parameters.Add("@TokenHash", token.TokenHash);
            parameters.Add("@CreatedAt", token.CreatedAt);
            parameters.Add("@ExpiredAt", token.ExpiredAt);
            parameters.Add("@IsUsed", token.IsUsed);

            return Connection.Execute(
                "USP_PASSWORD_RESET_INSERT",
                parameters,
                Transaction,
                commandType: CommandType.StoredProcedure);
        }
        public int ValidateAndUsePasswordResetToken(string tokenHash, string newPasswordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TokenHash", tokenHash);
            parameters.Add("@NewPassword", newPasswordHash);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            Connection.Execute(
                "USP_PASSWORD_RESET_VALIDATE_AND_USE",
                parameters,
                Transaction,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@Result"); // 1 sukses, 0 gagal
        }
        public List<UserUploadResponse> GetUserUploadValidationByTempCode(string tempCode)
        {
            DynamicParameters _dynamicParams = new(new
            {
                TempCode = tempCode
            });

            var data = Connection.Query<UserUploadResponse>("USP_GET_UPLOADED_USER_DOWNLOAD",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            return data;
        }




    }
}
