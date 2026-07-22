using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_SERVICE_PARTY")]
    public partial class ServiceParty
    {
        [Key]
        public int ID { get; set; }
        public string SERVICE_PARTY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}