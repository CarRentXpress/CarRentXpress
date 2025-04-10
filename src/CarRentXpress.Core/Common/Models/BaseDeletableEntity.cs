namespace CarRentXpress.Data.Entities;

public class BaseDeletableEntity<TKey> : IBaseDeletableEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(+3); //eest
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(+3); //eest
    public DateTime? DeletedAt { get; set; } 
    public bool IsDeleted { get; set; } = false;
}