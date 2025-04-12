using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarRentXpress.Data.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [StringLength(10, MinimumLength = 10)]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "EGN must be a 10-digit number")]
    public string EGN { get; set; } // EGN must be unique!

}