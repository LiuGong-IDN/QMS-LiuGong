using Dapper;
using Dapper.Contrib.Extensions;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;
using System.Data;
using System.Text.Json;
using CommandType = System.Data.CommandType;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface ICustomerRepository : IDapperRepository<CustomerRequest>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, string companyCode, string companyName, object parameter = null);
        int Save(CustomerRequest customer, long userId);
        int Delete(string id);
        int Update(CustomerRequest customer, long userId);
        CustomerMaintenance GetById(long id);
        int ValidateUploadProcess(List<CustomerUpload> data, long userId, out string tempCode, out string msgError);
        DataSourceResult GetUploadCustomerTemporary(DataSourceRequest request, object parameter = null);
        int SubmitFinalUpload(string tempCode, long userId, out string msgError);

        List<CustomerUploadResponse> GetCustomerUploadValidationByTempCode(string tempCode);
    }

    public partial class CustomerRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<CustomerRequest>(connection, transaction), ICustomerRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string companyCode, string companyName, object parameter = null)
        {
            string orderBy = string.Empty;
            string orderType = string.Empty;

            if (request.Sorts.Count > 0)
            {
                if (request.Sorts[0].SortDirection.ToString() == "Descending")
                {
                    orderBy = request.Sorts[0].Member;
                    orderType = "DESC";
                }
                else if (request.Sorts[0].SortDirection.ToString() == "Ascending")
                {
                    orderBy = request.Sorts[0].Member;
                    orderType = "ASC";
                }
            }
            else
            {
                orderBy = "CREATED_DATE";
                orderType = "DESC";
            }

            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetPaged",

                Companycode = companyCode,
                Companyname = companyName,

                RowSkip = (request.Page - 1) * request.PageSize,
                PageSize = request.PageSize,
                OrderBy = orderBy,
                OrderType = orderType
            });

            var data = Connection.Query<CustomerMaintenance>("USP_GET_CUSTOMER",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                Companycode = companyCode,
                Companyname = companyName,
            });

            int total = Connection.ExecuteScalar<int>("USP_GET_CUSTOMER",
                    _dynamicParams,
                        commandTimeout: 30,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

            return new DataSourceResult { Data = data, Total = total };
        }

        public int Save(CustomerRequest customer, long userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", customer.ID); 
            parameters.Add("@CompanyName", customer.COMPANY_NAME);
            parameters.Add("@Address", customer.ADDRESS);
            parameters.Add("@CompanyCode", customer.COMPANY_CODE);
            parameters.Add("@By", userId);
            parameters.Add("@IsValid", false, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute("USP_CUSTOMER_SAVE", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@IsValid") ? 1 : 0;
        }

        public int Update(CustomerRequest customer, long userId) => Save(customer, userId);

        public int Delete(string id)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_CUSTOMER_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }

        public CustomerMaintenance GetById(long id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "Getlist");
            parameters.Add("@ID", id);

            return Connection.QueryFirstOrDefault<CustomerMaintenance>(
                "USP_GET_CUSTOMER",
                parameters,
                commandType: CommandType.StoredProcedure,
                transaction: Transaction
            );
        }

        public int ValidateUploadProcess(List<CustomerUpload> data, long userId, out string tempCode, out string msgError)
        {
            var isValid = false;
            msgError = string.Empty;
            tempCode = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var json = JsonSerializer.Serialize(data);

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@Data", json);
            parameters.Add("@UserId", userId);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
            parameters.Add("@MsgError", msgError, DbType.String, ParameterDirection.Output, 4000);

            Connection.Execute("dbo.USP_CUSTOMER_UPLOAD", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("@IsValid");
            msgError = parameters.Get<string>("@MsgError");

            return isValid ? 1 : 0;
        }
        public DataSourceResult GetUploadCustomerTemporary(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<CustomerUpload>();

            return output;
        }

        public int SubmitFinalUpload(string tempCode, long userId, out string msgError)
        {
            var isValid = false;
            msgError = string.Empty;

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@UserId", userId);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
            parameters.Add("@MsgError", msgError, DbType.String, ParameterDirection.Output, 4000);

            Connection.Execute("dbo.USP_CUSTOMER_UPLOAD_SUBMIT", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("@IsValid");
            msgError = parameters.Get<string>("@MsgError");

            return isValid ? 1 : 0;
        }

        public List<CustomerUploadResponse> GetCustomerUploadValidationByTempCode(string tempCode)
        {
            DynamicParameters _dynamicParams = new(new
            {
                TempCode = tempCode
            });

            var data = Connection.Query<CustomerUploadResponse>("USP_GET_UPLOADED_CUSTOMER_DOWNLOAD",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            return data;
        }
    }
}
