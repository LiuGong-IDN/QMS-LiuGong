using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_MENU")]
    public partial class Menu
    {
        [Key]
        public int ID { get; set; }
        public int? PARENT_ID { get; set; }
        public string TITLE { get; set; }
        public string URL { get; set; }
        public string DESCRIPTION { get; set; }
        public string ICON_CLASS { get; set; }
        public int ORDER_INDEX { get; set; }
        public bool STATUS { get; set; }
        public string FUNCTION_ID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string? UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
