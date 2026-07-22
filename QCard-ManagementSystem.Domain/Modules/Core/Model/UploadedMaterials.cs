namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class UploadedMaterials
    {
        public int ID { get; set; }
        public string TEMPCODE { get; set; }
        public int PART_ID { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public int QTY { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}