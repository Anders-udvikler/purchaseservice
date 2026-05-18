

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
    public string? personId { get; set; }

    [BsonElement("title")]
    public string? title { get; set; }

    [BsonElement("description")]
    public string? description { get; set; }

    [BsonElement("size")]
    public string? size { get; set; }

    [BsonElement("quantity")]
    public string? quantity { get; set; }

    [BsonElement("price")]
    public int price { get; set; }

    [BsonElement("condition")]
    public string? condition { get; set; }

    [BsonElement("zip_code")]
    public string? zip_code { get; set; }

    [BsonElement("status")]
    public Status? status { get; set; }

    [BsonElement("color")]
    public Color? color { get; set; }

    [BsonElement("categories")]
    public List<Category>? categories { get; set; }

    [BsonElement("images")]
    public List<Image>? Images { get; set; } = new List<Image>();
}
}