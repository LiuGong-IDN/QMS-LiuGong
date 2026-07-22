using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_SYSTEM")]
    public partial class Config
    {
        [Key]
        public string SYSTEM_ID { get; set; }
        [Key]
        public string SYSTEM_TYPE { get; set; }
        public string SYSTEM_VALUE_TEXT { get; set; }
        public DateTime SYSTEM_VALUE_DT { get; set; }
        public int SYSTEM_VALUE_NUM { get; set; }
        public string SYSTEM_DESC { get; set; }
        public DateTime CREATED_DT { get; set; }
        public DateTime CHANGED_DT { get; set; }
        public string CREATED_BY { get; set; }
        public string CHANGED_BY { get; set; }
    }
}
