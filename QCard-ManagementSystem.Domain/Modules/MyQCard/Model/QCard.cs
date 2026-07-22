using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.MyQCard.Model
{
    [Table("TB_R_QCARD")]
    public partial class QCard
    {
        [Key]
        public int ID { get; set; }
        public string DOCUMENT_APPROVAL_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public int VALID_TO_MONTH { get; set; }
        public int VALID_TO_YEAR { get; set; }
        public int CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}