using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBikeAPI.Models;

public class Inventory
{
    [Key]
    public int InventoryId { get; set; }

    public int ProductId { get; set; }

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "in_stock"; // Các trạng thái: in_stock, sold, warranty

    // Khóa ngoại liên kết tới bảng Product
    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
}