using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_SETTING")]
    public partial class Setting
    {
        public string SETTING_ID { get; set; }
        public string SETTING_TYPE { get; set; }
        public string SETTING_VALUE { get; set; }
        public string SETTING_DESC { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string? UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
