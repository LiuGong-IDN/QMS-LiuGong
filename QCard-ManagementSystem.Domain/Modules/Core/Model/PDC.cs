using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_PDC")]
    public partial class PDC
    {
        public int ID { get; set; }
        public string PDC_NAME { get; set; }
        public string CITY { get; set; }
        public string PROVINCE { get; set; }
        public string FULL_ADDRESS { get; set; }
        public bool IS_ACTIVE { get; set; }
        public int ESTABLISH_YEAR { get; set; }
        public int ESTABLISH_QUARTER { get; set; }
        public int CAPACITY {  get; set; }
        public int NOTE { get; set; }
    }
}