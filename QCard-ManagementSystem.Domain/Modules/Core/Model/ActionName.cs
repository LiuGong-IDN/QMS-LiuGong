using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_SYSTEM")]
    public partial class ActionName
    {
        public string SYSTEM_TYPE { get; set; }
        public string SYSTEM_VALUE_TEXT { get; set; }
        public string SYSTEM_DESC { get; set; }

    }
}