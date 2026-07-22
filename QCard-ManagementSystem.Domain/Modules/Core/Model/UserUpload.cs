using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_USER_UPLOAD")]
    public partial class UserUpload
    {
        public string ACTION { get; set; }
        public int NO { get; set; }
        public string TEMPCODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string COMPANY_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string ADDRESS { get; set; }
        public string ROLE_NAME { get; set; }
        public string PDC { get; set; }
        public string STATUS { get; set; }
        public string MESSAGE { get; set; }
    }
}
