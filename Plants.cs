using System.ComponentModel.DataAnnotations;

namespace ASP.NET_PlantsOrder.Models
{
    public class Plants
    {
        [Key]
        public int PlantId { get; set; }

        [Required]
        [StringLength(100)]
        public string PlantName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, 1000)]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}