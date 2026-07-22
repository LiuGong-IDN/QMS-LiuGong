using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    [Table("VW_M_ROLE_PERMISSION")]
    public partial class RolePermissionView
    {
        public int ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string FUNCTION_ID { get; set; }
        public bool CREATE_FEATURE { get; set; }
        public bool UPDATE_FEATURE { get; set; }
        public bool DELETE_FEATURE { get; set; }
        public bool UPLOAD_FEATURE { get; set; }
        public bool DOWNLOAD_FEATURE { get; set; }
        public bool VIEW_FEATURE { get; set; }
        public bool SEARCH_FEATURE { get; set; }
    }
}
