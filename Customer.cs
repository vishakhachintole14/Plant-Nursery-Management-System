
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_PlantsOrder.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public bool IsAdmin { get; set; } = false;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
