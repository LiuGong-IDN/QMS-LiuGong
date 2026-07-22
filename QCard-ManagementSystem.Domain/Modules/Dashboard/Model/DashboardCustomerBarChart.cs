using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.Model
{
    public partial class DashboardCustomerBarChart
    {
        public string PROGRAM_NAME { get; set; }
        public int CLAIMED_QTY { get; set; }
        public int PACKAGE_QTY {  get; set; }
    }
}