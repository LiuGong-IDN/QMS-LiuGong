using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.ViewModel
{
    [Table("VW_DASHBOARD_ADMIN_SUMMARY")]
    public partial class DashboardAdminSummary
    {
        public int TOTAL_QCARD { get; set; }
        public int TOTAL_SERIAL { get; set; }
        public int TOTAL_CLAIM {  get; set; }
    }
}