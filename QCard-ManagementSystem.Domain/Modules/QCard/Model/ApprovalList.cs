using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class ApprovalList
    {
        [Key]
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public DateTime TOPUP_DATE { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string CARD_NO { get; set; }
        public int VALID_TO_MONTH { get; set; }
        public int VALID_TO_YEAR { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public int FORM_TYPE { get; set; }
        public int DOCUMENT_APPROVAL_ID { get; set; }
        public string CATEGORY_PROGRAM { get; set; }
        public int STATUS_ID { get; set; } = 0;
        public string STATUS_NAME { get; set; }
    }
}