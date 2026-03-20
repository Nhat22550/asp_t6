using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // 1. Thêm thư viện Schema này

namespace EBikeAPI.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    // 2. Thêm quy định kiểu decimal(18,2) cho giá tiền
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [StringLength(50)]
    public string Color { get; set; } = string.Empty;
}