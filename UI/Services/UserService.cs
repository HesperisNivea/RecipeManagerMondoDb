using DataAccess.Services;
using UI.Models;

namespace UI.Services;

public class UserService
{
    UserRepository _userRepository = new UserRepository();

    public List<UserModel> GetAllUsers()
    {
        var users = _userRepository.GetAllUsers().ToList().Select( user => new UserModel()
        {
            Id = user.Id,
            UserName = user.UserName,

        }).ToList();

        return users;
    }
}