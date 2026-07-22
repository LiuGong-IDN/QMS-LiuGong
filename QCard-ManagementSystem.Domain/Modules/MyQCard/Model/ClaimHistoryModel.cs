using Modules.MyMachines.Model;

namespace Modules.MyQCard.Model
{
    public partial class ClaimHistoryModel
    {
        public int CLAIM_ID { get; set; }
        public int PDC_ID { get; set; }
        public string CLAIM_NO { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string SERIAL_NO { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime CLAIM_DATE { get; set; }
        public string PDC_NAME { get; set; }
        public string STATUS {  get; set; }
        public bool IsOpened { get; set; }
        public bool IsOpenedByCust { get; set; }
        public bool IsOpenedByAdmin { get; set; }

        public DateTime LATEST_UPDATED_DATE { get; set; }
    }
}