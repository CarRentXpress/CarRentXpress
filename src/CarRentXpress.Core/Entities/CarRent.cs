using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarRentXpress.Data.Entities;

public class CarRent : CarRent<string>
{
    public CarRent()
    {
        Id = Guid.NewGuid().ToString();
    }
}

public class CarRent<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}