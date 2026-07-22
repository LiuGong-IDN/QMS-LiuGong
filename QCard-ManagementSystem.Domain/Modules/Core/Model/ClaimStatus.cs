using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class ClaimStatus
    {
        public int ID { get; set; }
        public string STATUS_NAME { get; set; }
        public bool IsActive { get; set; }
    }
}