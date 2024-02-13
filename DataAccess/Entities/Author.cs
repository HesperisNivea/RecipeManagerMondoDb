using MongoDB.Bson;

namespace DataAccess.Entities;

public class Author 
{
    public ObjectId UserId { get; set; } 
    public string UserName { get; set; }
}