using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    [Table("VW_M_MASTER_PROGRAM")]
    public partial class ProgramView
    {
        public long ID { get; set; }
        public string PROGRAM_NO { get; set; }
        public string MACHINE_TYPE { get; set; }
        public string PRODUCT_MODEL { get; set; }
        public int MODEL_ID { get; set; }
        public string MODEL_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

    }
}