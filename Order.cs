
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_PlantsOrder.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string OrderStatus { get; set; } = "Confirmed";

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Customer? Customer { get; set; }
    }
}
