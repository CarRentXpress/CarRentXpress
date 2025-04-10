using System.ComponentModel.DataAnnotations;
using CarRentXpress.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRentXpress.Application.Entities;

public class CarRentDto : BaseDeletableEntity<string>
{
    public CarRentDto()
    {
        Id = Guid.NewGuid().ToString();
    }

    [Required]
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