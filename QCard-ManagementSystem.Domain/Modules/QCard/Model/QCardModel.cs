using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    [Table("TB_R_QCARD")]
    public partial class QCardModel
    {
        [Key]
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