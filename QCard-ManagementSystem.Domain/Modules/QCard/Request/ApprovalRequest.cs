using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Request
{
    public class ApprovalRequest
    {
        public int FORM_TYPE_ID { get; set; }
        public List<string> DOCUMENT_APPROVAL_IDS { get; set; }
        public string ACTION_NAME { get; set; }
        public string EVIDENCE_FILE_NAME { get; set; }
        public string NOTES {  get; set; }
        public string REJECT_REASON { get; set; }
    }
}
