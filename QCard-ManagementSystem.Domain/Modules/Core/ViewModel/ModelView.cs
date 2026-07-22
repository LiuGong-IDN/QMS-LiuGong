using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    [Table("VW_M_MODEL")]
    public partial class ModelView
    {
        public int ID { get; set; }
        public string MODEL_CODE { get; set; }
        public string MODEL_NAME { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}