namespace Modules.MyMachines.Model
{
    public partial class MachinesDetail
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string MACHINE_TYPE { get; set; }
        public string MODEL { get; set; }
        public string IMAGE_PATH { get; set; }
        public DateTimeOffset PURCHASE_DATE { get; set; }
        public string SERVICE_PARTY { get; set; }
        public DateTimeOffset CREATED_DATE { get; set; }
        public decimal CURRENT_AMOUNT { get; set; }
    }
}