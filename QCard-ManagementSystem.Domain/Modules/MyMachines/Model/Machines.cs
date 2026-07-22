namespace Modules.MyMachines.Model
{
    public partial class MachinesDownload
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string MACHINE_TYPE { get; set; }
        public string MODEL { get; set; }
        public DateTimeOffset PURCHASE_DATE { get; set; }
        public string SERVICE_PARTY { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public int TOTAL_PACKAGE_QTY { get; set; }
        public int TOTAL_CLAIMED_QTY { get; set; }
        public decimal INITIAL_BALANCE { get; set; }
        public decimal USED_BALANCE { get; set; }
        public decimal REMAINING_BALANCE { get; set; }
    }
}