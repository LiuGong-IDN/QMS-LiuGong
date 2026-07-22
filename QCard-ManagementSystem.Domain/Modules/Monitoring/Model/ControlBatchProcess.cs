using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QCard.ManagementSystem.Domain.Modules.Monitoring.Model
{
    [Table("TB_M_CTL_BATCH_PROC")]
    public partial class ControlBatchProcess
    {
        [Key]
        public string FUNCTION { get; set; }
        public DateTime LAST_PROCESS { get; set; }
        public string DESCRIPTION { get; set; }
        public string STATUS { get; set; }
        public string IP_ADDRESS { get; set; }
        public string DEST_FOLDER { get; set; }
        public string TYPE { get; set; }
        public string USER_ID { get; set; }
        public string PASSWORD { get; set; }
    }
}
