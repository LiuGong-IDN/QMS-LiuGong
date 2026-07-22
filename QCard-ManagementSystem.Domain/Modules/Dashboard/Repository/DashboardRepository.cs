using Dapper;
using Kendo.Mvc.UI;
using Modules.Dashboard.Model;
using Modules.Dashboard.ViewModel;
using Modules.MyMachines.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using System.Data;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IDashboardRepository : IDapperRepository<DashboardAdminSummary>
    {
        DashboardAdminSummary GetDashboardAdminSummary();
        List<DashboardAdminLineChartStatistic> GetDashboardAdminQCardStatistics(string currentMonth);
        List<DashboardAdminLineChartStatistic> GetDashboardAdminClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev);
        List<DashboardListModel> GetDashboardMostClaimedProgram(string selectedDate);
        List<DashboardListModel> GetDashboardTopCustomer(string selectedDate);
        List<DashboardListModel> GetDashboardTopProductGroup(string selectedDate);
        List<DashboardListModel> GetDashboardTopProductModel(string selectedDate);

        DashboardCustomerSummary GetDashboardCustomerSummary(long userId);
        List<LatestUpdatedClaim> GetDashboardCustomerLatestClaim(long userId);
        List<DashboardCustomerBarChart> GetDashboardCustomerRemainingClaim(string serialNo, long customerId);

        DashboardPDCSummary GetDashboardPDCSummary(long pdcId);
        List<DashboardLineChartStatistic> GetDashboardPDCClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev, long pdcId);
        List<DashboardListModel> GetDashboardPDCMostClaimedProgram(string selectedDate, long pdcId);
        List<DashboardListModel> GetDashboardPDCTopCustomer(string selectedDate, long pdcId);

    }

    public partial class DashboardRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<DashboardAdminSummary>(connection, transaction), IDashboardRepository
    {
        #region Repository Dashboard Admin
        public DashboardAdminSummary GetDashboardAdminSummary()
        {
            DashboardAdminSummary? summary = FindAll<DashboardAdminSummary>("VW_DASHBOARD_ADMIN_SUMMARY").FirstOrDefault() ?? new DashboardAdminSummary();
            return summary;
        }
        
        public List<DashboardAdminLineChartStatistic> GetDashboardAdminQCardStatistics(string currentMonth)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@CurrentMonth", currentMonth);

            List<DashboardAdminLineChartStatistic> result = [.. Connection.Query<DashboardAdminLineChartStatistic>(
                "USP_GET_DASHBOARD_ADMIN_QCARD_STATISTIC",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }

        public List<DashboardAdminLineChartStatistic> GetDashboardAdminClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@StartDateCurrent", startDateCurrent);
            GetListParams.Add("@EndDateCurrent", endDateCurrent);
            GetListParams.Add("@StartDatePrev", startDatePrev);
            GetListParams.Add("@EndDatePrev", endDatePrev);

            List<DashboardAdminLineChartStatistic> result = [.. Connection.Query<DashboardAdminLineChartStatistic>(
                "USP_GET_DASHBOARD_ADMIN_CLAIM_STATISTIC",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public List<DashboardListModel> GetDashboardMostClaimedProgram(string selectedDate)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@SelectedMonth", selectedDate);

            List<DashboardListModel> result = [.. Connection.Query<DashboardListModel>(
                "USP_GET_DASHBOARD_ADMIN_MOST_CLAIMED_PROGRAM",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public List<DashboardListModel> GetDashboardTopCustomer(string selectedDate)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@SelectedMonth", selectedDate);

            List<DashboardListModel> result = [.. Connection.Query<DashboardListModel>(
                "USP_GET_DASHBOARD_ADMIN_TOP_CUSTOMER",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public List<DashboardListModel> GetDashboardTopProductGroup(string selectedDate)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@SelectedMonth", selectedDate);

            List<DashboardListModel> result = [.. Connection.Query<DashboardListModel>(
                "USP_GET_DASHBOARD_ADMIN_TOP_PRODUCT_GROUP",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public List<DashboardListModel> GetDashboardTopProductModel(string selectedDate)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@SelectedMonth", selectedDate);

            List<DashboardListModel> result = [.. Connection.Query<DashboardListModel>(
                "USP_GET_DASHBOARD_ADMIN_TOP_PRODUCT_MODEL",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        #endregion

        #region Repository Dashboard Customer
        public DashboardCustomerSummary GetDashboardCustomerSummary(long userId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", userId);

            var result = Connection.QuerySingleOrDefault<DashboardCustomerSummary>(
                "USP_GET_DASHBOARD_CUSTOMER_SUMMARY",
                param: param,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return result;
        }
        public List<LatestUpdatedClaim> GetDashboardCustomerLatestClaim(long userId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", userId);

            List<LatestUpdatedClaim> result = [.. Connection.Query<LatestUpdatedClaim>(
                "USP_GET_DASHBOARD_CUSTOMER_LATEST_CLAIM",
                param: param,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];

            return result;
        }
        public List<DashboardCustomerBarChart> GetDashboardCustomerRemainingClaim(string serialNo, long customerId)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@SerialNo", serialNo);
            GetListParams.Add("@CustomerId", customerId);

            List<DashboardCustomerBarChart> result = [.. Connection.Query<DashboardCustomerBarChart>(
                "USP_GET_DASHBOARD_CUSTOMER_REMAINING_CLAIM",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        #endregion

        #region Dashboard PDC
        public DashboardPDCSummary GetDashboardPDCSummary(long pdcId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PDC", pdcId);

            DashboardPDCSummary? result = Connection.Query<DashboardPDCSummary>(
                "USP_GET_DASHBOARD_PDC_SUMMARY",
                parameters,
                null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            ).FirstOrDefault();

            return result;
        }
        public List<DashboardLineChartStatistic> GetDashboardPDCClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev, long pdcId)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@StartDateCurrent", startDateCurrent);
            GetListParams.Add("@EndDateCurrent", endDateCurrent);
            GetListParams.Add("@StartDatePrev", startDatePrev);
            GetListParams.Add("@EndDatePrev", endDatePrev);
            GetListParams.Add("@PdcId", pdcId);

            List<DashboardLineChartStatistic> result = [.. Connection.Query<DashboardLineChartStatistic>(
                "USP_GET_DASHBOARD_PDC_CLAIM_STATISTIC",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public List<DashboardListModel> GetDashboardPDCMostClaimedProgram(string selectedDate, long pdcId)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@SelectedMonth", selectedDate);
            GetListParams.Add("@PdcId", pdcId);

            List<DashboardListModel> result = [.. Connection.Query<DashboardListModel>(
                "USP_GET_DASHBOARD_PDC_MOST_CLAIMED_PROGRAM",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public List<DashboardListModel> GetDashboardPDCTopCustomer(string selectedDate, long pdcId)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@SelectedMonth", selectedDate);
            GetListParams.Add("@PdcId", pdcId);

            List<DashboardListModel> result = [.. Connection.Query<DashboardListModel>(
                "USP_GET_DASHBOARD_PDC_TOP_CUSTOMER",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        #endregion
    }
}