

using Categories;
using Colors;
using GraphProductView.Models;
using Images;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Subcategories;

namespace Furnitures
{
public class Furniture
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("sales_post_guid")]
    public string? SalesPostGuid { get; set; }

    [BsonElement("guid")]
    public string? Guid { get; set; }

    [BsonElement("personId")]
    public string? PersonId { get; set; }

    [BsonElement("title")]
    public string? Title { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("size")]
    public string? Size { get; set; }

    [BsonElement("quantity")]
    public string? Quantity { get; set; }

    [BsonElement("price")]
    public int Price { get; set; }

    [BsonElement("condition")]
    public string? Condition { get; set; }

    [BsonElement("zip_code")]
    public string? ZipCode { get; set; }

    [BsonElement("status")]
    public Status? Status { get; set; }

    [BsonElement("color")]
    public Color? Color { get; set; }

    [BsonElement("categories")]
    public List<Category>? Categories { get; set; }

    [BsonElement("images")]
    public List<Image>? Images { get; set; } = new List<Image>();
}
}