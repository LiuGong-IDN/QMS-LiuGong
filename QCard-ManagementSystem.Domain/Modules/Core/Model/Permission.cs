using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_PERMISSION")]
    public partial class Permission
    {
        [Key]
        public int ID { get; set; }
        [Key]
        public int ROLE_ID { get; set; }
        [Key]
        public int FUNCTION_ID { get; set; }
        public bool CREATE_FEATURE { get; set; }
        public bool UPDATE_FEATURE { get; set; }
        public bool DELETE_FEATURE { get; set; }
        public bool UPLOAD_FEATURE { get; set; }
        public bool DOWNLOAD_FEATURE { get; set; }
        public bool VIEW_FEATURE { get; set; }
        public bool SEARCH_FEATURE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string? UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
