using System.ComponentModel.DataAnnotations;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class QCardPrintModel
    {
        public List<QcardData> qcardDatas { get; set; }
        public string frontSVGPath { get; set; }
        public string backSVGPath { get; set; }
    }
    public class QcardData
    {
        public int ID { get; set; }
        public int? CUSTOMER_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CARD_NO { get; set; }
        public string VALID_TO_MONTH { get; set; }
        public string VALID_TO_YEAR { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}