using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.ViewModel
{
    [Table("VW_DASHBOARD_CUSTOMER_SUMMARY")]
    public partial class DashboardCustomerSummary
    {
        public int TOTAL_MACHINE { get; set; }
        public int TOTAL_CLAIM {  get; set; }
        public int DAYS_LEFT {  get; set; }
    }
}