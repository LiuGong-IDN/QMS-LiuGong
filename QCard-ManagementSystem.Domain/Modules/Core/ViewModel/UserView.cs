using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    [Table("VW_M_USER")]
    public partial class UserView
    {
        public int ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string EMAIL { get; set; }
        public string COMPANY { get; set; }
        public long COMPANY_ID { get; set; }
        public string CARD_NO { get; set; }
        public string ROLE_ID { get; set; }
        public long PDC_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string PDC { get; set; }
        public string PASSWORD { get; set; }
        public bool PASSWORD_RESET { get; set; }
        public string DEPOT_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}