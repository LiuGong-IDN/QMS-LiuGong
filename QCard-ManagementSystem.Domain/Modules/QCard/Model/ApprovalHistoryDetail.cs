using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class ApprovalHistoryDetail
    {
        public long ID {  get; set; }
        public string REQUESTED_BY { get; set; }
        public string SUBMITTED_DATE { get; set; }
        public string STATUS_NAME { get; set; }
        public string EVIDENCE_1 { get; set; }
        public string REASON {  get; set; }
        public string NOTES { get; set; }
        public string LAST_ACTION { get; set; }
    }
}