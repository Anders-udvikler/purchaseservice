using Furnitures;
using Users;

public class FurnitureId
{
    public string EventType { get; set; } = string.Empty;
    public string Guid { get; set; } = string.Empty;
    public string PersonGUID { get; set; } = string.Empty;
    public Furniture ListingDetails { get; set; } = new();
    public User UserDetails { get; set; } = new();
}