namespace Modules.MyMachines.Model
{
    public class MachineInfo
    {
        public int ID { get; set; }
        public int PDC { get; set; }
        public List<PDCEmail> EMAIL_LIST { get; set; }
        public string SERIAL_NO { get; set; }
        public string CLAIM_NO { get; set; }
        public string COMPANY_NAME { get; set; }
        public string CLAIM_TYPE_NAME { get; set; }
    }

    public class PDCEmail
    {
        public string EMAIL { get; set; }
        public string PDC_NAME { get; set; }
    }
}