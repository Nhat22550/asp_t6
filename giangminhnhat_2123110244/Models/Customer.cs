using System.ComponentModel.DataAnnotations;

namespace EBikeAPI.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
}