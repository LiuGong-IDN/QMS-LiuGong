using System.ComponentModel.DataAnnotations.Schema;
namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class ProgramMaterialUpload
    {
        public string TEMPCODE { get; set; }
        public int NO { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public int QTY { get; set; }
        public int ORDER_INDEX { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
        public string ACTION { get; set; }
    }
}