using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.QCard.ViewModel
{
    [Table("VW_R_QCARD_LIST")]
    public partial class QCardListView
    {
        public int ID { get; set; }
        public string CARD_NO { get; set; }
        public int VALID_TO_MONTH { get; set; }
        public int VALID_TO_YEAR { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string COMPANY_NAME { get; set; }
    }
}