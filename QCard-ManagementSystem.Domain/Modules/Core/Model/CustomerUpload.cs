using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_CUSTOMER_UPLOAD")]
    public partial class CustomerUpload
    {
        public int NO { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
        public string ACTION { get; set; }
    }
}
