using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_ASSET_TYPE")]
    public partial class AssetType
    {
        [Key]
        public int ID { get; set; }
        public string ASSET_TYPE_CODE { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
    }
}