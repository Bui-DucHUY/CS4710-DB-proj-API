namespace APIs.Models
{
    public class ClinicService
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public decimal Fee { get; set; }
        public string CPTCode { get; set; }
    }
}