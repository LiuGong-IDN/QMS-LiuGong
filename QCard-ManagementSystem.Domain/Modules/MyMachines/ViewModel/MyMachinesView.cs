using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.MyMachines.ViewModel
{
    public partial class MyMachinesView
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string MACHINE_TYPE {  get; set; }
        public string MODEL { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public string SERVICE_PARTY { get; set; }
        public int AVAILABLE_PROGRAM { get; set; }
        public decimal INITIAL_BALANCE { get; set; }
        public decimal USED_BALANCE { get; set; }
        public decimal REMAINING_BALANCE { get; set; }
        public bool  IS_PROGRAM { get; set; }
        public bool  IS_VOUCHER { get; set; }
    }
}