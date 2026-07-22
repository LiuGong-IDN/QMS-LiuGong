using System.Text.Json;
using System.Text.RegularExpressions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Validator;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class ModelService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : DbService(db)
    {
        private readonly IEmailService _emailService = emailService;
        private readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(30);
        private readonly IWebHostEnvironment _env = env;
        private readonly long _userId = SessionManager.Id;

        public DataSourceResult GetDataSourceResult(DataSourceRequest request)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.ModelRepository.GetDataSourceResult(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicResponse Save(ModelRequest model)
        {
            string methodName = "Save";
            BasicResponse message = new();

            try
            {
                ModelValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(model);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                var result = Db.ModelRepository.Save(model, _userId);

                if (result > 0)
                {
                    message.Success = true;
                    message.Validate = true;
                    message.Message = string.Empty;

                    new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                }
                else
                {
                    throw new Exception("Failed to save data");
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Validate = true;
                message.Message = ex.Message;
            }

            return message;
        }

        public BasicResponse Delete(string id)
        {
            string methodName = "Delete";
            BasicResponse message = new();
            try
            {
                var result = Db.ModelRepository.Delete(id);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Delete Data");
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
        public ModelView GetModelById(string id, string modelCode,int assetType)
        {
            var empty = string.Empty;
            return Db.ModelRepository.ListModel("GetList", id, modelCode, assetType).FirstOrDefault();
        }
        public bool isSQLInjectString(string strWords)
        {
            strWords = Regex.Replace(strWords, @"[^\u0000-\u007F]+", string.Empty);

            bool isSQLInjection = false;
            List<string> badChars = new List<string>() { "select", "drop", ";", "--", "insert", "delete", "xp_", "¿", "waitfor delay" };

            for (int i = 0; i < badChars.Count(); i++)
            {
                if (strWords.Contains(badChars[i]))
                    isSQLInjection = true;
                break;
            }

            return isSQLInjection;
        }
    }
}
