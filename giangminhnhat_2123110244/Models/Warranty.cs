using System.ComponentModel.DataAnnotations;

namespace EBikeAPI.Models;

public class Warranty
{
    [Key]
    public int WarrantyId { get; set; }

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;
}