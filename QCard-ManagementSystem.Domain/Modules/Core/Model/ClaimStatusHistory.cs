using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class ClaimStatusHistory
    {
        public string STATUS { get; set; }
        public string PIC { get; set; }
        public DateTime DATE { get; set; }
    }
}