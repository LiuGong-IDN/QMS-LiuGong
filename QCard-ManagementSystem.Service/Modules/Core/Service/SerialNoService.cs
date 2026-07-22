using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.Core.Validator;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Modules.Core.Service
{
    public class SerialNoService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : DbService(db)
    {
        private readonly IEmailService _emailService = emailService;
        private readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(30);
        private readonly IWebHostEnvironment _env = env;
        private readonly string _userEmail = SessionManager.Email;

        public DataSourceResult GetDataSourceResult(DataSourceRequest request)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.SerialNoRepository.GetDataSourceResult(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public DataSourceResult GetSerialNoListByMachineId(DataSourceRequest request, int machineId)
        {
            string methodName = "GetSerialNoListByMachineId";
            var result = new DataSourceResult();
            try
            {
                result = Db.SerialNoRepository.GetSerialNoListByMachineId(request, machineId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public ProgramHeaderDetail GetMachineDetails(int programId)
        {
            string methodName = "GetMachineDetails";
            ProgramHeaderDetail result = new();
            try
            {
                result = Db.SerialNoRepository.GetProgramDetails(programId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicResponse Save(MachineRequest machine)
        {
            var methodName = "Save";
            BasicResponse message = new();
            var by = SessionManager.Id;
            try
            {
                SerialNoValidator validator = new();
                var validate = validator.Validate(machine);

                if (!validate.IsValid)
                {
                    message.Success = false;
                    message.Validate = false;
                    message.Message = JsonSerializer.Serialize(validate.Errors);
                    return message;
                }

                var result = Db.SerialNoRepository.Save(machine, by);

                if (result > 0)
                {
                    message.Success = true;
                    message.Validate = true;
                    message.Message = $"Data saved successfully";
                    new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.Validate = true;
                message.Message = ex.Message ?? "Failed to save.";
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return message;
        }
        public BasicResponse Delete(string id)
        {
            string methodName = "Delete";
            BasicResponse message = new();
            try
            {
                var result = Db.SerialNoRepository.Delete(id);

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
        public SerialNoView GetMachineByID(long id)
        {
            var empty = string.Empty;
            return Db.SerialNoRepository.ListMachine("GetList", id, empty).FirstOrDefault();
        }
        public BasicDataResponse GetSerialNoDetailsEdit(int id)
        {
            string methodName = "GetSerialNoDetailsEdit";
            var result = new BasicDataResponse();
            try
            {
                MachineDetail? serialNoDetail = Db.SerialNoRepository.GetSerialNoDetail(id);

                result.Data = serialNoDetail;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public BasicDataResponse GetSerialNoDetail(int id)
        {
            string methodName = "GetSerialNoDetail";
            var result = new BasicDataResponse();
            try
            {
                MachineDetail? serialNoDetail = Db.SerialNoRepository.GetSerialNoDetail(id);

                result.Data = serialNoDetail;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
    }
}
