using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBikeAPI.Models;

public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string Method { get; set; } = string.Empty; // Cash, Transfer, QR

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    [ForeignKey("OrderId")]
    public Order? Order { get; set; }
}