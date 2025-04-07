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
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Description { get; set; }
}