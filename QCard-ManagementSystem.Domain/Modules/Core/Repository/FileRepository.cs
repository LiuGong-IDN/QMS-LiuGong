using Dapper;
using Kendo.Mvc.UI;
using System.Data;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IFileRepository : IDapperRepository<FileModel>
    {
        int Save(FileModel fileModel, string[] columns);
        int Update(FileModel fileModel, string[] columns);
        int Delete(FileModel fileModel);
        BasicResponse SaveData(FileModel fileModel, string[] columns);
    }

    public partial class FileRepository : DapperRepository<FileModel>, IFileRepository
    {
        public FileRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }

        public int Save(FileModel fileModel, string[] columns)
        {
            var fileModelObj = (object)Mapping(fileModel);
            return Connection.Execute(DynamicQuery.GetInsertQuery("TB_R_FILE", fileModelObj, "ID", columns), fileModelObj, Transaction);
        }

        public int Update(FileModel fileModel, string[] columns)
        {
            var fileModelObj = (object)Mapping(fileModel);
            return Connection.Execute(DynamicQuery.GetUpdateQuery("TB_R_FILE", fileModelObj, "ID", columns), fileModelObj, Transaction);
        }

        public int Delete(FileModel fileModel)
        {
            var fileModelObj = (object)Mapping(fileModel);
            return Connection.Execute(DynamicQuery.GetHardDeleteQuery("TB_R_FILE", "ID"), fileModelObj, Transaction);
        }

        public BasicResponse SaveData(FileModel fileModel, string[] columns)
        {
            BasicResponse result = new BasicResponse();

            var fileModelObj = (object)Mapping(fileModel);
            var id = Connection.ExecuteScalar(DynamicQuery.GetInsertQuery("TB_R_FILE", fileModelObj, "ID", columns), fileModelObj, Transaction);

            result.Success = true;
            result.Message = id.ToString();

            return result;
        }

    }
}