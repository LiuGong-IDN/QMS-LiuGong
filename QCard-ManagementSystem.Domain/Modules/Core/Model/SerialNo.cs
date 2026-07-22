using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_MACHINE")]
    public partial class SerialNo
    {
        [Key]
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string ASSET_TYPE_ID { get; set; }
        public string MODEL_ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string PURCHASE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}