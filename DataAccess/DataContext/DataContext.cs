using DataAccess.Entities;
using MongoDB.Driver;

namespace DataAccess.DataContext;

public class DataContext // can be removed without consequences if not implemented
{
    private readonly IMongoCollection<Recipe> _recipes;

    public DataContext() // Create DataAccess class to create a singleton? 
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "RecipeManagerDb";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _recipes = database.GetCollection<Recipe>("Recipes", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }
}