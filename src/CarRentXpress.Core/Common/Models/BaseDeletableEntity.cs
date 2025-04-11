namespace CarRentXpress.Data.Entities;

public class BaseDeletableEntity<TKey> : IBaseDeletableEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(+3); //eest
    private DateTime _updatedAt = DateTime.UtcNow.AddHours(+3); //eest

    public DateTime UpdatedAt
    {
        get => _updatedAt;
        set => _updatedAt = value + TimeSpan.FromHours(+3);
    }
    private DateTime? _deletedAt;
    public DateTime? DeletedAt
    {
        get => _deletedAt;
        set => _deletedAt = value + TimeSpan.FromHours(+3);
    }
    public bool IsDeleted { get; set; } = false;
}