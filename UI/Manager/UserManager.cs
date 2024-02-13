using System.Windows;
using Common.DTOs;
using DataAccess.Services;

namespace UI.Manager;

public static class UserManager
{
    private static readonly UserRepository _userRepository = new UserRepository();

    private static List<UserRecord> _users = new List<UserRecord>();

    public static List<UserRecord> Users { get; set; }
    

	private static UserRecord _currentUser;

    public static UserRecord CurrentUser
    {
		get { return _currentUser; }
        set
        {
            _currentUser = value;
            CurrentUserChanged.Invoke();
        }
	}

    public static Action CurrentUserChanged;

    public static void LogIn(string name, string password)
    {
        if (!Users.ToList().Exists(newUser => newUser.UserName == name))
        {
            return;
        }

        if (Users.FirstOrDefault(newUser => newUser.UserName == name).Password != password)
        {
            return;
        }

        var user = Users.First(savedUser => savedUser.UserName == name);
        CurrentUser = user;
        CurrentUserChanged.Invoke();

    }

    public static void LogOut()
    {
        CurrentUser = null;
        CurrentUserChanged.Invoke();

    }

    public static void Register(string name, string password)
    {
        if (Users.ToList().Exists(newUser => newUser.UserName == name))
        {
            MessageBox.Show("This Username is taken!");
            return; 
        }

        var newUser = new UserRecord("", name, password);
        _userRepository.AddUser(newUser);

        Users = _userRepository.GetAllUsers(); // this way newUser is added with its id to Users list
        CurrentUserChanged.Invoke();
    }
}