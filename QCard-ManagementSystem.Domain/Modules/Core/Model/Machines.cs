namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class Machines
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string MACHINE_TYPE { get; set; }
        public string PRODUCT_MODEL { get; set; }
        public string COMPANY_NAME { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public string SERVICE_PARTY_NAME { get; set; }
        public string CATEGORY_CLAIM { get; set; }
        public int CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}