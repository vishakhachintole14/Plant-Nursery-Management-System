using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ASP.NET_PlantsOrder.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Plants>? Plants { get; set; }
    }
}