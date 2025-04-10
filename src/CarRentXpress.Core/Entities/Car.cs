using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarRentXpress.Data.Entities;

public class Car : Car<string>
{
    public Car()
    {
        Id = Guid.NewGuid().ToString();
    }
}

public class Car<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}