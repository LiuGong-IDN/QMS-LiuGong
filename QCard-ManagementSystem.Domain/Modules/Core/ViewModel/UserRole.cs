using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    [Table("VW_M_USER_ROLE")]
    public partial class UserRoleView
    {
        public int USER_ID { get; set; }
        public int ROLE_ID { get; set; }
        public string ID { get; set; }
        public string REG_NO { get; set; }
        public string ROLE_NAME { get; set; }
        public int USER_OD {  get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CHANGED_BY { get; set; }
        public DateTime CHANGED_DATE { get; set; }
    }
}
