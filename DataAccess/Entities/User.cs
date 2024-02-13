using MongoDB.Bson;

namespace DataAccess.Entities;

public class User
{
    public ObjectId Id { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

}