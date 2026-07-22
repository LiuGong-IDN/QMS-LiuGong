using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public partial class ExcelResponse
    {
        public bool canSubmit { get; set; }
        public bool isSaved { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string tempCode { get; set; }
    }
}
