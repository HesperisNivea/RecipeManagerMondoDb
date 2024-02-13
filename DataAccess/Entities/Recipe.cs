using Common.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAccess.Entities;

public class Recipe
{
    public ObjectId Id { get; set; }
    public string Title { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public string Desctiption { get; set; }
    public List<string> Instructions { get; set; }
    public Author Author { get; set; }

    [BsonRepresentation(BsonType.String)]
    public List<Category> Categories { get; set; }
    public int VoteCount { get; set; }
    public float RatingSum { get; set; }

   // public float RatingAverage { get; set; } //RatingSum / VoteCount;
}