namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class ProgramModel
    {
        public int ID { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public int MODEL_ID { get; set; }
        public string MODEL_NAME { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public string MACHINE_TYPE { get; set; }
        public int PRIORITY_ID { get; set; }
        public string PRIORITY { get; set; }
        public string IMAGE_PATH { get; set; }
        public int CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public int PROGRAM_ID { get; set; }
        public int PART_ID { get; set; }
        public string PART_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public int QTY { get; set; }
    }

    public partial class ProgramHeaderDetail
    {
        public ProgramModel Header { get; set; }
        public List<ProgramDetail> Detail { get; set; }
    }

    public partial class ProgramDetail
    {
        public int ID { get; set; }
        public string PART_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public int QTY { get; set; }
    }
}