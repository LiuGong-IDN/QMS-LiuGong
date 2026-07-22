namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class ProgramMaterial
    {
        public int ID { get; set; }
        public int PART_ID { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public int QTY { get; set; }
        public int ORDER_INDEX { get; set; }
    }
}