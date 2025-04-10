using System.ComponentModel.DataAnnotations;
using CarRentXpress.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRentXpress.DTOs;

public class CarDto : BaseDeletableEntity<string>
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int Seats { get; set; }
    public string? Description { get; set; }
    [Precision(18,2)]
    public decimal PricePerDay { get; set; }
    public string ImgUrl { get; set; }
}