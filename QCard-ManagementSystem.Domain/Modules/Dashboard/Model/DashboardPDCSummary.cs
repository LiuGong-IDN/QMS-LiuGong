using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.Model
{
    public partial class DashboardPDCSummary
    {
        public int SUBMITTED { get; set; }
        public int REJECTED { get; set; }
        public int PENDING {  get; set; }
        public int PARTIAL_PICKUP {  get; set; }
        public int PREPARING {  get; set; }
        public int READY_FOR_PICKUP {  get; set; }
    }
}