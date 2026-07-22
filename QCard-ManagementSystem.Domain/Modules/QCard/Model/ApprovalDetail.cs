using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class ApprovalDetail
    {
        [Key]
        public int ID { get; set; }
        public int TOPUP_ID { get; set; }
        public string CARD_NO { get; set; }
        public int DOCUMENT_APPROVAL_ID { get; set; }
        public int FORM_TYPE { get; set; }
        public int STATUS_ID { get; set; }
        public int STATUS_NAME { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMPANY_NAME { get; set; }
        public int VALID_TO_MONTH { get; set; }
        public int VALID_TO_YEAR { get; set; }
        public string VALID_THRU { get; set; }
        public DateTime TOPUP_DATE { get; set; }
        public string SERIAL_NO { get; set; }
        public int MACHINE_ID { get; set; }
        public decimal VOUCHER_AMOUNT { get; set; }
        public decimal CURRENT_AMOUNT { get; set; }
        public bool IS_PROGRAM { get; set; }
        public bool IS_VOUCHER { get; set; }
        public string PJB_NO { get; set; }
        public string EVIDENCE_PJB { get; set; }
        public string CATEGORY_PROGRAM { get; set; }
        public string PRODUCT_GROUP { get; set; }
        public string PRODUCT_MODEL { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public int? APPROVAL_LEVEL_ID { get; set; }
        public string EVIDENCE_1 { get; set; }
        public string EVIDENCE_2 { get; set; }
        public string EVIDENCE_3 { get; set; }
        public string NOTES_1 { get; set; }
        public string NOTES_2 { get; set; }
        public string NOTES_3 { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}