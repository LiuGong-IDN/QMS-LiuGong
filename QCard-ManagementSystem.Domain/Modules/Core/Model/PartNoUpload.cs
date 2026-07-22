using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_PARTNO_UPLOAD")]
    public partial class PartNoUpload
    {
        [Key]
        public string ACTION { get; set; }
        public int NO { get; set; }
        public string TEMPCODE { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public int RETAIL_PRICE { get; set; }
        public string STATUS { get; set; }
        public string MESSAGE { get; set; }
    }
}