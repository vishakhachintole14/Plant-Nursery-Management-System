using System.ComponentModel.DataAnnotations;

public class Supplier
{
    [Key]
    public int SupplierId { get; set; }

    [Required]
    public string SupplierName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }
}