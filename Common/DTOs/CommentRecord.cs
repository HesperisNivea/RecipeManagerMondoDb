namespace Common.DTOs;

public record CommentRecord(string Id,string Title, string Content, int Rating, DateTime Published, string RecipeId, string AuthorId, string AuthorName);