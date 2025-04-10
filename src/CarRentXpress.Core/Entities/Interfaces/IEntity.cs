namespace CarRentXpress.Data.Entities;

public interface IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}