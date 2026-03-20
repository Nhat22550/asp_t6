using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBikeAPI.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Khóa ngoại liên kết tới bảng Customer
    [ForeignKey("CustomerId")]
    public Customer? Customer { get; set; }
}