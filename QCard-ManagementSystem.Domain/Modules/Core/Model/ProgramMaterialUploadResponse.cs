namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class ProgramMaterialUploadResponse
    {
        public string TEMPCODE { get; set; }
        public int NO { get; set; }
        public string PART_NO { get; set; }
        public string UPLOADED_PART_NAME { get; set; }
        public string REGISTERED_PART_NAME { get; set; }
        public string PART_NAME_MATCH_STATUS { get; set; }
        public int QTY { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
        public string ACTION { get; set; }
    }
}