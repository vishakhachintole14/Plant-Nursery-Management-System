
using System;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_PlantsOrder.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = "COD";

        public string PaymentStatus { get; set; } = "Pending";

        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
