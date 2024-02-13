using Common.DTOs;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class UserRepository
{

    private readonly IMongoCollection<User> _users;

    public UserRepository() // Create DataAccess class to create a singleton? 
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "RecipeManagerDb";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _users = database.GetCollection<User>("Users", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }

    public List<UserRecord> GetAllUsers()
    {
        var filter = Builders<User>.Filter.Empty;

        var allUsers = _users.Find(filter).ToList().Select( u => new UserRecord(
            u.Id.ToString(),
            u.UserName,
            u.Password)).ToList();

        return allUsers;
    }

    public void AddUser(UserRecord userRecord)
    {
        var newUser = new User()
        {
            UserName = userRecord.UserName,
            Password = userRecord.Password,
        };
  
        _users.InsertOne(newUser);
    }

    public void RemoveUser(UserRecord userRecord)
    {
        var filter = Builders<User>.Filter
            .Eq("_id", ObjectId.Parse(userRecord.Id));

        _users.DeleteOne(filter);
    }

    public void UpdateUser(UserRecord userRecord) 
    {
        var filter = Builders<User>.Filter
            .Eq("_id", ObjectId.Parse(userRecord.Id));

        var update = Builders<User>.Update.Set(user => user.UserName, userRecord.UserName).Set(user => user.Password, userRecord.Password);

        _users.UpdateOne(filter, update);
   
    }
}