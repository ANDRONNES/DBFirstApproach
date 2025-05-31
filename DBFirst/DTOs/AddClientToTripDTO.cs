using System.ComponentModel.DataAnnotations;

namespace DBFirst.DTOs;

public class AddClientToTripDTO
{
    [Required]
    [MaxLength(120)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(120)]
    public string LastName { get; set; }
    [Required]
    [MaxLength(120)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MaxLength(120)]
    public string Telephone { get; set; }
    [Required]
    [MaxLength(11)]
    public string Pesel { get; set; }
    [Required]
    public int IdTrip { get; set; }
    [Required]
    [MaxLength(120)]
    public string TripName { get; set; }
    [Required]
    public DateTime PaymentDate { get; set; }
}