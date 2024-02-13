using Common.Enums;

namespace Common.DTOs;

public record RecipeRecord(string Id,
    string Title,
    List<IngredientRecord> Ingredients,
    string Description,
    List<string> Instructions,
    AuthorRecord AuthorRecord,
    List<Category> Categories,
    int VoteCount,
    float RatingSum,float AverageRating); 