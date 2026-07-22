using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.Model
{
    public partial class DashboardListModel
    {
        public string TEXT { get; set; }
        public int VALUE { get; set; }
    }
}