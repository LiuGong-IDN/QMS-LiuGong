using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_PARTNO")]
    public partial class PartNo
    {
        [Key]
        public int ID { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public int MODEL_ID { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public decimal RETAIL_PRICE { get; set; }
    }
}