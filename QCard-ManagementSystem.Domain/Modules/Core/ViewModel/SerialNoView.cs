using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    [Table("VW_M_SERIAL_NO")]
    public partial class SerialNoView
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public string CATEGORY_CLAIM { get; set; }
        public string COMPANY_NAME { get; set; }
        public DateTime CREATED_DATE { get; set; }

    }
}