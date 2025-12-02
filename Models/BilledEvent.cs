using System;

namespace APIs.Models
{
    public class BilledEvent
    {
        public int EventID { get; set; }
        public int ServiceID { get; set; }
        public int ProviderID { get; set; }
        public DateTime DateOfService { get; set; }
        public decimal BilledAmount { get; set; }
    }
}