using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.Model
{
    public partial class DashboardAdminLineChartStatistic
    {
        public string STATE { get; set; }
        public int DAYS { get; set; }
        public string DAY_TEXT { get; set; }
        public int VALUE {  get; set; }
    }
}