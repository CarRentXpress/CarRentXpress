using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarRentXpress.Data.Entities;

public class Car : BaseDeletableEntity<string>
{
    public Car()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    [Required]
    public string Brand { get; set; }
    [Required]
    public string Model { get; set; }
    [Required]
    public int Year { get; set; }
    [Required]
    public int Seats { get; set; }
    public string? Description { get; set; }
    [Precision(18,2)]
    public decimal PricePerDay { get; set; }
    [Required]
    public string ImgUrl { get; set; }
}