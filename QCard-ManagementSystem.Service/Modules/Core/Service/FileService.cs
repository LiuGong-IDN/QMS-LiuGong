using FluentFTP.Helpers;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using OBS;
using OBS.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Validator;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class FileService : DbService
    {
        //OBS Parameter
        protected ObsSDK _sdk;
        protected string _obsEndPoint;
        protected string _obsAccessKey;
        protected string _obsSecretKey;
        protected string _obsBucketName;

        public FileService(IDbHelper db) : base(db)
        {
            this._obsEndPoint = new ConfigService(db).GetConfigByIdAndType("OBS_PARAMETER", "OBS_END_POINT").SYSTEM_VALUE_TEXT;
            this._obsAccessKey = new ConfigService(db).GetConfigByIdAndType("OBS_PARAMETER", "OBS_ACCESS_KEY").SYSTEM_VALUE_TEXT;
            this._obsSecretKey = new ConfigService(db).GetConfigByIdAndType("OBS_PARAMETER", "OBS_SECRET_KEY").SYSTEM_VALUE_TEXT;
            this._obsBucketName = new ConfigService(db).GetConfigByIdAndType("OBS_PARAMETER", "OBS_BUCKET_NAME").SYSTEM_VALUE_TEXT;

            ObsConfig config = new ObsConfig();
            config.Endpoint = _obsEndPoint;
            config.SecurityProtocolType = System.Net.SecurityProtocolType.Tls12;
            config.AuthType = AuthTypeEnum.OBS;
            var client = new ObsClient(_obsAccessKey, _obsSecretKey, config);
            this._sdk = new ObsSDK(client);
        }

        public byte[] GetFileBytes(string objectKey)
        {
            string methodName = "GetFileBytes";
            byte[] result = null;

            try
            {
                var objectName = objectKey.Replace(_obsEndPoint, string.Empty);
                result = _sdk.GetByteObject(_obsBucketName, objectName);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }

        public string GetObjectKeyFileName(string objectKey)
        {
            var objectKeyArray = objectKey.Split("/");
            return objectKeyArray[objectKeyArray.Length - 1];
        }

        public string GetFileMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            var contentType = provider.TryGetContentType(fileName, out var type)
                ? type
                : "application/octet-stream";

            return contentType;
        }

        public BasicResponse UploadFile(IFormFile file, string moduleName)
        {
            string methodName = "UploadFile";
            var result = new BasicResponse();

            result.Success = false;

            try
            {
                var fileFolderName = moduleName.Replace("Service", string.Empty);
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);
                var obsFileName = $"{fileFolderName}/{fileNameWithoutExt}{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var obsObjectKey = _obsEndPoint + obsFileName;

                //add to obs
                using (Stream fileStream = file.OpenReadStream())
                {
                    _sdk.PutObject(_obsBucketName, obsFileName, fileStream);
                }

                //add to database
                var fileData = new FileModel()
                {
                    OBJECT_KEY = obsObjectKey,
                    FILE_NAME = file.FileName,
                    FILE_MIMETYPE = file.ContentType,
                    FILE_SIZE = Convert.ToInt32(file.Length)
                };

                var saveFile = Save(fileData);
                if (!saveFile.Success)
                {
                    //Fail to save DB 
                    //Delete Obs File
                    _sdk.DeleteObject(_obsBucketName, obsFileName, null);
                    throw new Exception("Failed Update Data");
                }
                else
                {
                    result = saveFile;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }

        public BasicResponse Save(FileModel file)
        {
            string methodName = "Save";
            BasicResponse message = new BasicResponse();

            try
            {
                var fileData = new FileModel()
                {
                    OBJECT_KEY = file.OBJECT_KEY,
                    FILE_NAME = file.FILE_NAME,
                    FILE_MIMETYPE = file.FILE_MIMETYPE,
                    FILE_SIZE = file.FILE_SIZE,
                    CREATED_DATE = DateTime.Now,
                    CREATED_BY = SessionManager.Current,
                };

                message = Db.FileRepository.SaveData(fileData, new string[]
                {
                    "OBJECT_KEY",
                    "FILE_NAME",
                    "FILE_MIMETYPE",
                    "FILE_SIZE",
                    "CREATED_DATE",
                    "CREATED_BY"
                });

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

        public BasicResponse Delete(long fileId)
        {
            string methodName = "Delete";
            BasicResponse message = new BasicResponse();

            try
            {
                var fileData = Db.FileRepository.Find(new { ID = fileId }).FirstOrDefault();

                if (fileData == null)
                {
                    message.Success = false;
                    message.Message = "File tidak ditemukan.";
                    return message;
                }

                var obsFileName = fileData.OBJECT_KEY.Replace(_obsEndPoint, string.Empty);
                _sdk.DeleteObject(_obsBucketName, obsFileName, null);

                int result = Db.FileRepository.Delete(fileData);
                if (result > 0)
                {
                    message.Success = true;
                    message.Message = "File berhasil dihapus";
                }
                else
                {
                    throw new Exception("Gagal menghapus file dari database.");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, GetType().Name);
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return message;
        }
    }
}
