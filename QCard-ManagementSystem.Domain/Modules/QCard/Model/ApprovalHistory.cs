using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class ApprovalHistory
    {
        [Key]
        public int ID { get; set; }
        public int DOCUMENT_APPROVAL_ID { get; set; }
        public int TOPUP_ID { get; set; }
        public int ROLE_ID { get; set; }
        public int APPROVAL_LEVEL_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public DateTime SUBMITTED_DATE { get; set; }
        public string REQUESTED_BY { get; set; }
    }
}