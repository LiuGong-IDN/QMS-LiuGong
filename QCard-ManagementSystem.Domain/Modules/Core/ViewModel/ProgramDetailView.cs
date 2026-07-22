using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    [Table("VW_M_PROGRAM_DETAIL")]
    public partial class ProgramDetailView
    {
        public int ID { get; set; }
        public string PART_NO { get; set; }
        public string QTY { get; set; }
        public string PART_NAME { get; set; }
    }
}