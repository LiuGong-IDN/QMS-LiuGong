using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.MyQCard.ViewModel
{
    [Table("VW_R_QCARD")]
    public partial class QCardView
    {
        public string CUSTOMER_NAME { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string EMAIL { get; set; }
        public string CARD_NO { get; set; }
        public string COMPANY { get; set; }
        public int VALID_TO_MONTH { get; set; }
        public int VALID_TO_YEAR { get; set; }
    }
}