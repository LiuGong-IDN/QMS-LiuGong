using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class ClaimProcessStatus
    {
        public int STATUS_TO { get; set; }
        public string STATUS_TO_NAME { get; set; }
        public string ACTION_NAME { get; set; }
    }
}