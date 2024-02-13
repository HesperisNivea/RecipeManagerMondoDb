using Common.DTOs;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class CommentRepository
{

    private readonly IMongoCollection<Comment> _comments;

    private readonly RecipeRepository _recipes = new RecipeRepository();

    public CommentRepository() // Create DataAccess class to create a singleton? 
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "RecipeManagerDb";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _comments = database.GetCollection<Comment>("Comments", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }

    public IEnumerable<CommentRecord> GetAllCommentRecords()
    {
        var filter = Builders<Comment>.Filter.Empty;

        var allcomments = _comments.Find(filter).ToList().Select(c => new CommentRecord
        (
            c.Id.ToString(),
            c.Titel,
            c.Content,
            c.Rating,
            c.Published,
            c.RecipeId.ToString(),
            c.AuthorId.ToString(),
            c.AuthorName
        ));

        return allcomments;
    }
    public IEnumerable<CommentRecord> GetAllCommentsRelatedToRecipe(string RecipeId)
    {
        var filter = Builders<Comment>.Filter.Eq("RecipeId", ObjectId.Parse(RecipeId));

        var allcomments = _comments.Find(filter).ToList().Select(c => new CommentRecord
        (
            c.Id.ToString(),
            c.Titel,
            c.Content,
            c.Rating,
            c.Published,
            c.RecipeId.ToString(),
            c.AuthorId.ToString(),
            c.AuthorName
            ));

        return allcomments;
    }

    public void AddComment(CommentRecord commentRecord)
    {
        var newComment = new Comment()
        {
            Titel = commentRecord.Title,
            Content = commentRecord.Content,
            Rating = commentRecord.Rating,
            Published = commentRecord.Published,
            RecipeId = ObjectId.Parse(commentRecord.RecipeId),
            AuthorId = ObjectId.Parse(commentRecord.AuthorId),
            AuthorName = commentRecord.AuthorName
            
        };
        _recipes.AddRating(commentRecord.RecipeId, commentRecord.Rating);
        _comments.InsertOne(newComment);
    }

    public void UpdateComment(CommentRecord commentRecord)
    {
        var filter = Builders<Comment>.Filter.Eq("_id", ObjectId.Parse(commentRecord.Id));

        var update = Builders<Comment>.Update.Set(comment => comment.Titel, commentRecord.Title)
            .Set(comment => comment.Content, commentRecord.Content)
            .Set(comment => comment.Rating, commentRecord.Rating)
            .Set(comment => comment.Published, commentRecord.Published)
            .Set(comment => comment.RecipeId, ObjectId.Parse(commentRecord.RecipeId));

        _comments.UpdateOne(filter, update);

    }

    public void DeleteComment(CommentRecord commentRecord)
    {
        var filter = Builders<Comment>.Filter.Eq("_id", ObjectId.Parse(commentRecord.Id));

        _recipes.RemoveRating(commentRecord.RecipeId,commentRecord.Rating);
        _comments.DeleteOne(filter);

    }

    public void DeleteAllRecipesComments(string recipeId)
    {
        var filter = Builders<Comment>.Filter.Eq("RecipeId", ObjectId.Parse(recipeId));

        _comments.DeleteMany(filter);
    }
}