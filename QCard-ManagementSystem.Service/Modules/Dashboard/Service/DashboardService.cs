using DocumentFormat.OpenXml.Office2010.Excel;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Modules.Dashboard.Model;
using Modules.Dashboard.ViewModel;
using Modules.MyMachines.Model;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.Core.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

namespace QCard.ManagementSystem.Service.Modules.Dashboard.Service
{
    public class DashboardService(IDbHelper db) : DbService(db)
    {
        private readonly long _pdcId = SessionManager.PdcId;
        #region Service Dashboard Admin
        public BasicDataResponse GetDashboardAdminSummary()
        {
            string methodName = "GetDashboardAdminSummary";
            var result = new BasicDataResponse();
            try
            {
                DashboardAdminSummary? data = Db.DashboardRepository.GetDashboardAdminSummary();

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardAdminQCardStatistics(string currentMonth)
        {
            string methodName = "GetDashboardAdminQCardStatistics";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardAdminLineChartStatistic> data = Db.DashboardRepository.GetDashboardAdminQCardStatistics(currentMonth);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardAdminClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev)
        {
            string methodName = "GetDashboardAdminClaimStatistics";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardAdminLineChartStatistic> data = Db.DashboardRepository.GetDashboardAdminClaimStatistics(startDateCurrent, endDateCurrent, startDatePrev, endDatePrev);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardMostClaimedProgram(string selectedDate)
        {
            string methodName = "GetDashboardMostClaimedProgram";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardListModel> data = Db.DashboardRepository.GetDashboardMostClaimedProgram(selectedDate);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardTopCustomer(string selectedDate)
        {
            string methodName = "GetDashboardTopCustomer";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardListModel> data = Db.DashboardRepository.GetDashboardTopCustomer(selectedDate);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardTopProductGroup(string selectedDate)
        {
            string methodName = "GetDashboardTopProductGroup";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardListModel> data = Db.DashboardRepository.GetDashboardTopProductGroup(selectedDate);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardTopProductModel(string selectedDate)
        {
            string methodName = "GetDashboardTopProductModel";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardListModel> data = Db.DashboardRepository.GetDashboardTopProductModel(selectedDate);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        #endregion

        #region Service Dashboard Customer
        public BasicDataResponse GetDashboardCustomerSummary()
        {
            string methodName = "GetDashboardCustomerSummary";
            var result = new BasicDataResponse();
            var userId = SessionManager.Id;
            try
            {
                DashboardCustomerSummary? data = Db.DashboardRepository.GetDashboardCustomerSummary(userId);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardCustomerLatestClaim()
        {
            string methodName = "GetDashboardCustomerLatestClaim";
            var result = new BasicDataResponse();
            var userId = SessionManager.Id;
            try
            {
                List<LatestUpdatedClaim> data = Db.DashboardRepository.GetDashboardCustomerLatestClaim(userId);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardCustomerRemainingClaim(string serialNo)
        {
            string methodName = "GetDashboardCustomerRemainingClaim";
            var result = new BasicDataResponse();
            var customerId = SessionManager.CompanyId;
            try
            {
                List<DashboardCustomerBarChart> data = Db.DashboardRepository.GetDashboardCustomerRemainingClaim(serialNo, customerId);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        #endregion

        #region Service Dashboard PDC
        public BasicDataResponse GetDashboardPDCSummary()
        {
            string methodName = "GetDashboardPDCSummary";
            var result = new BasicDataResponse();
            try
            {
                DashboardPDCSummary? data = Db.DashboardRepository.GetDashboardPDCSummary(_pdcId);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardPDCClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev)
        {
            string methodName = "GetDashboardPDCClaimStatistics";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardLineChartStatistic> data = Db.DashboardRepository.GetDashboardPDCClaimStatistics(startDateCurrent, endDateCurrent, startDatePrev, endDatePrev, _pdcId);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public BasicDataResponse GetDashboardPDCMostClaimedProgram(string selectedDate)
        {
            string methodName = "GetDashboardPdcMostClaimedProgram";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardListModel> data = Db.DashboardRepository.GetDashboardPDCMostClaimedProgram(selectedDate, _pdcId);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetDashboardPDCTopCustomer(string selectedDate)
        {
            string methodName = "GetDashboardPDCTopCustomer";
            var result = new BasicDataResponse();
            try
            {
                List<DashboardListModel> data = Db.DashboardRepository.GetDashboardPDCTopCustomer(selectedDate, _pdcId);

                result.Data = data;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        #endregion
    }
}