using MongoDB.Bson;

namespace DataAccess.Entities;

public class Comment
{
    public ObjectId Id { get; set; }

    public string Titel { get; set; }

    public string Content { get; set; }

    public int Rating { get; set; }

    public DateTime Published { get; set; }

    public ObjectId RecipeId { get; set; }

    public ObjectId AuthorId { get; set; }

    public string AuthorName { get; set; }
}