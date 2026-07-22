using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Modules.MyQCard.Model;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.QCard.Validator;
using System.Text.Json;

namespace QCard.ManagementSystem.Service.Modules.MyQCard.Service
{
    public class ClaimProcessService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : DbService(db)
    {
        private readonly IEmailService _emailService = emailService;
        private readonly long _userId = SessionManager.Id;
        private readonly long _UserCompanyId = SessionManager.CompanyId;
        private readonly long _pdcId = SessionManager.PdcId;

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string claimNo, string serialNo, int status)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.ClaimProcessRepository.GetDataSourceResult(request, _pdcId, claimNo, serialNo, status);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public BasicDataResponse GetClaimProcessDetail(string claimNo)
        {
            string methodName = "GetClaimProcessDetail";
            var result = new BasicDataResponse();
            try
            {
                ClaimProcessDetail claimDetail = Db.ClaimProcessRepository.GetClaimProcessDetail(_userId, claimNo);

                result.Data = claimDetail;
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

        public BasicDataResponse GetClaimProcessVoucherDetail(string claimNo)
        {
            string methodName = "GetClaimProcessVoucherDetail";
            var result = new BasicDataResponse();
            try
            {
                ClaimProcessDetail claimDetail = Db.ClaimProcessRepository.GetClaimProcessVoucherDetail(_userId,claimNo);

                result.Data = claimDetail;
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

        public BasicResponse UpdateClaimProcess(ClaimProcessRequest request)
        {
            string methodName = "UpdateClaimProcess";
            BasicResponse message = new();
            try
            {
                UpdateClaimProcessValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(request);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                var result = Db.ClaimProcessRepository.UpdateClaimProcess(request, _userId);

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

        public BasicResponse VerifyClaimProcess(ClaimProcessVerifyRequest request, int claimId)
        {
            string methodName = "VerifyClaimProcess";
            BasicResponse message = new();
            try
            {
                var result = Db.ClaimProcessRepository.VerifyClaimProcess(request, claimId, _userId);

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

        public BasicResponse UpdatePartClaimQty(ClaimProcessUpdatePartQty request)
        {
            string methodName = "UpdatePartClaimQty";
            BasicResponse message = new();
            try
            {
                var result = Db.ClaimProcessRepository.UpdatePartClaimQty(request, _userId);

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

        public BasicResponse ClaimProcessUpdatePartial(ClaimProcessUpdatePartialRequest request)
        {
            string methodName = "UpdatePartClaimQty";
            BasicResponse message = new();
            try
            {
                //validator
                ClaimProcessValidator dataValidator = new(request);
                var validatorResult = dataValidator.Validate(request);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                var result = Db.ClaimProcessRepository.ClaimProcessUpdatePartial(request, _userId);

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