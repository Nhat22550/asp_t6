using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBikeAPI.Models;

public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    // Khóa ngoại liên kết tới bảng Order
    [ForeignKey("OrderId")]
    public Order? Order { get; set; }
}