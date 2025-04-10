using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarRentXpress.Data.Entities;

public class Car : Entity<string>
{
    public Car()
    {
        this.Id = Guid.NewGuid().ToString();
    }
    [Required]
    public string Brand { get; set; }
    [Required]
    public string Model { get; set; }
    [Precision(18,2)]
    public decimal PricePerDay { get; set; }
    [Required]
    public string ImgUrl { get; set; }
}