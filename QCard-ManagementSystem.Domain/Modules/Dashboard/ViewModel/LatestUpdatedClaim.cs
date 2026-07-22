using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Dashboard.ViewModel
{
    [Table("VW_DASHBOARD_CUSTOMER_LATEST_CLAIM")]
    public partial class LatestUpdatedClaim
    {
        public string CLAIM_NO { get; set; }
        public string STATUS { get; set; }
        public DateTime UPDATED_DATE {  get; set; }
    }
}