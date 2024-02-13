using Common.DTOs;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using MongoDB.Bson.Serialization;

namespace DataAccess.Services;

public class RecipeRepository
{
    private readonly IMongoCollection<Recipe> _recipes;


    public RecipeRepository()  
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "RecipeManagerDb";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _recipes = database.GetCollection<Recipe>("Recipes", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }

    public IEnumerable<RecipeRecord> GetAllRecipes()
    {
        var filter = Builders<Recipe>.Filter.Empty;
        var allRecipes = _recipes.Find(filter).ToList().Select(r => new RecipeRecord
        (
                r.Id.ToString(),
                    r.Title,
                    r.Ingredients.Select(i =>
                        new IngredientRecord
                        (
                            i.Measurement, 
                            i.IngredientName)
                        ).ToList(),
                    r.Desctiption,
                    r.Instructions,
                    new AuthorRecord(r.Author.UserId.ToString(), r.Author.UserName),
                    r.Categories,
                    r.VoteCount,
                    r.RatingSum,
                r.RatingSum/r.VoteCount)
        );

        return allRecipes;
    }
    public void AddRecipe(RecipeRecord recipe)
    {
        var newRecipe = new Recipe()
        {
            Title = recipe.Title,
            Desctiption = recipe.Description,
            Ingredients = recipe.Ingredients.Select(ingredient =>
                    new Ingredient()
                    {
                        IngredientName = ingredient.IngredientName,
                        Measurement = ingredient.Measurement
                    })
                .ToList(),
            Instructions = recipe.Instructions,
            Author = new Author() { UserId = ObjectId.Parse(recipe.AuthorRecord.UserId), UserName = recipe.AuthorRecord.Name }, 
            Categories = recipe.Categories,
            VoteCount = recipe.VoteCount,
            RatingSum = recipe.RatingSum,

        };

        _recipes.InsertOne(newRecipe);
    }

    public void UpdateRecipe(RecipeRecord recipeRecord)
    {
        var filter = Builders<Recipe>.Filter
            .Eq("_id", ObjectId.Parse(recipeRecord.Id));
        
        var update = Builders<Recipe>.Update.Set(recipe => recipe.Title, recipeRecord.Title)  // VoteCount and RatingSum can be set only by adding a comment by other users than author
            .Set(recipe => recipe.Desctiption, recipeRecord.Description)
            .Set(recipe => recipe.Ingredients, recipeRecord.Ingredients.Select(ingredient =>
                new Ingredient()
                {
                    IngredientName = ingredient.IngredientName,
                    Measurement = ingredient.Measurement
                }).ToList())
            .Set(recipe => recipe.Instructions, recipeRecord.Instructions)
            .Set(recipe => recipe.Categories, recipeRecord.Categories);

        _recipes.UpdateOne(filter, update);
    }

    public void DeleteRecipe(RecipeRecord recipeRecord)
    {
        var filter = Builders<Recipe>.Filter
            .Eq("_id", ObjectId.Parse(recipeRecord.Id));

         _recipes.DeleteOne(filter);
    }

    public void AddRating(string recipeId, int rating) // flout commentRating?? 
    {
        var filter = Builders<Recipe>.Filter
            .Eq("_id", ObjectId.Parse(recipeId));

        var update = Builders<Recipe>.Update.Inc(recipe => recipe.VoteCount, 1)
            .Inc(recipe => recipe.RatingSum, rating);

        _recipes.UpdateOne(filter, update);

    }

    public void RemoveRating(string recipeId, int rating) 
    {
        
        var filter = Builders<Recipe>.Filter
            .Eq("_id", ObjectId.Parse(recipeId));

        var update = Builders<Recipe>.Update.Inc(recipe => recipe.VoteCount, -1)
            .Inc(recipe => recipe.RatingSum, -rating);

        _recipes.UpdateOne(filter, update);

        //ResetAverageRating(recipeId);
    }

    public void ResetAverageRating(string recipeId)
    {
        var filter = Builders<Recipe>.Filter
            .Eq("_id", ObjectId.Parse(recipeId));

        //var update = Builders<Recipe>.Update.Set(recipe => recipe.RatingAverage);
    }

    public List<RecipeRecord> GetAllUsersRecipes(string userId)
    {
        var filter = Builders<Recipe>.Filter.Eq(x => x.Author.UserId, ObjectId.Parse(userId));

        var allRecipes = _recipes.Find(filter).ToList().Select(r => new RecipeRecord
            (
                r.Id.ToString(),
                r.Title,
                r.Ingredients.Select(i =>
                    new IngredientRecord
                    (
                        i.Measurement,
                        i.IngredientName)
                ).ToList(),
                r.Desctiption,
                r.Instructions,
                new AuthorRecord(r.Author.UserId.ToString(), r.Author.UserName),
                r.Categories,
                r.VoteCount,
                r.RatingSum,
                r.RatingSum/r.VoteCount)
        ).ToList();

        return allRecipes;
    }
}