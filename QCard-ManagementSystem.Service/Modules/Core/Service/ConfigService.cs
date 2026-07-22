using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Infrastructure.Session;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.IO;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Service.Modules.Core.Validator;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class ConfigService : DbService
    {
        public ConfigService(IDbHelper db) : base(db)
        {
        }

        public Config GetConfigByIdAndType(string SystemID, string SystemType)
        {
            return Db.ConfigRepository.Find(new { SYSTEM_ID = SystemID, SYSTEM_TYPE = SystemType }).FirstOrDefault();
        }
        public Config GetConfigByIdAndValueText(string SystemID, string ValueText)
        {
            return Db.ConfigRepository.Find(new { SYSTEM_ID = SystemID, SYSTEM_VALUE_TEXT = ValueText }).FirstOrDefault();
        }
        public DataSourceResult GetDataSourceResult(DataSourceRequest request)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.ConfigRepository.GetDataSourceResult(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public BasicResponse Save(Config config)
        {
            string methodName = "Save";
            BasicResponse message = new BasicResponse();
            try
            {
                //validator
                ConfigValidator dataValidator = new ConfigValidator();
                var validatorResult = dataValidator.Validate(config);

                if (!validatorResult.IsValid) {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                var configData = new Config()
                {
                    SYSTEM_ID = config.SYSTEM_ID,
                    SYSTEM_TYPE = config.SYSTEM_TYPE,
                    SYSTEM_VALUE_TEXT = config.SYSTEM_VALUE_TEXT,
                    SYSTEM_VALUE_NUM = config.SYSTEM_VALUE_NUM,
                    SYSTEM_DESC = config.SYSTEM_DESC,
                    CREATED_DT = DateTime.Now,
                    CREATED_BY = SessionManager.Current,
                };

                var result = Db.ConfigRepository.Save(configData, new string[]
                {
                    "SYSTEM_ID",
                    "SYSTEM_TYPE",
                    "SYSTEM_VALUE_TEXT",
                    "SYSTEM_VALUE_NUM",
                    "SYSTEM_DESC",
                    "CREATED_DT",
                    "CREATED_BY"
                });

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Create Data");
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

        public BasicResponse Update(Config config)
        {
            string methodName = "Update";
            BasicResponse message = new BasicResponse();
            try
            {
                //validator
                ConfigValidator dataValidator = new ConfigValidator();
                var validatorResult = dataValidator.Validate(config);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                var configData = new Config()
                {
                    SYSTEM_ID = config.SYSTEM_ID,
                    SYSTEM_TYPE = config.SYSTEM_TYPE,
                    SYSTEM_VALUE_TEXT = config.SYSTEM_VALUE_TEXT,
                    SYSTEM_VALUE_NUM = config.SYSTEM_VALUE_NUM,
                    SYSTEM_DESC = config.SYSTEM_DESC,
                    CHANGED_DT = DateTime.Now,
                    CHANGED_BY = SessionManager.Current,
                };

                var result = Db.ConfigRepository.Update(configData, new string[]
                {
                    "SYSTEM_ID",
                    "SYSTEM_TYPE",
                    "SYSTEM_VALUE_TEXT",
                    "SYSTEM_VALUE_NUM",
                    "SYSTEM_DESC",
                    "CHANGED_DT",
                    "CHANGED_BY"
                });

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

        public BasicResponse Delete(string systemId, string systemType)
        {
            string methodName = "Delete";
            BasicResponse message = new BasicResponse();
            try
            {
                var configData = new Config()
                {
                    SYSTEM_ID = systemId,
                    SYSTEM_TYPE = systemType
                };

                var result = Db.ConfigRepository.Delete(configData);

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

        public BasicFileResponse DownloadTemplate()
        {
            BasicFileResponse file = new BasicFileResponse();
            string objectKey = new ConfigService(Db).GetConfigByIdAndType("OBS_OBJECT_KEY", "CONFIG_MAINTENANCE_TEMPLATE").SYSTEM_VALUE_TEXT;

            file.FileContent = new FileService(Db).GetFileBytes(objectKey);
            file.FileName = new FileService(Db).GetObjectKeyFileName(objectKey);
            file.FileMimeType = new FileService(Db).GetFileMimeType(file.FileName);

            return file;
        }
    }
}
