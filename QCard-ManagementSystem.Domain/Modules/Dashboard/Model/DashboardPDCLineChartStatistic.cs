using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.Model
{
    public partial class DashboardLineChartStatistic
    {
        public string STATE { get; set; }
        public int DAYS { get; set; }
        public string DAY_TEXT { get; set; }
        public int VALUE {  get; set; }
    }
}