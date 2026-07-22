using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class Topup
    {
        [Key]
        public int ID { get; set; }
        public int? CARD_ID { get; set; }
        public DateTime TOPUP_DATE { get; set; }
        public int? APPROVAL_LEVEL_ID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
    public partial class TopupDetail
    {
        [Key]
        public int TOPUP_ID { get; set; }
        public int? SERIAL_NO { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}