using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarRentXpress.Data.Entities;

public class CarRent : BaseDeletableEntity<string>
{
    public CarRent()
    {
        Id = Guid.NewGuid().ToString();
    }

    [Required]
    public string CarId { get; set; }
    public Car Car { get; set; }
    [Required]
    public ApplicationUser User { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Precision(18,2)]
    public decimal Price { get; set; }
}