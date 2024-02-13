using System.Windows;
using System.Windows.Controls;
using Common.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Services;
using UI.Manager;
using UI.Models;

namespace UI.ViewModels;

public class LoginViewModel : ObservableObject
{
    private string _registerUsername = String.Empty;

	public string RegisterUsername
	{
		get { return _registerUsername; }
        set
        {
            _registerUsername = value;
            OnPropertyChanged();
        }
        
	}

	private string _registerPassword = String.Empty;

	public string RegisterPassword
    {
		get { return _registerPassword; }
        set
        {
            _registerPassword = value;
            OnPropertyChanged();
        }
	}

	private string _loginUsername = String.Empty;

	public string LoginUsername
	{
		get { return _loginUsername; }
        set
        {
            _loginUsername = value;
            OnPropertyChanged();
        }
	}

	private string _loginPassword = String.Empty;

	public string LoginPassword
    {
        get { return _loginPassword; }
        set
        {
            _loginPassword = value;
            OnPropertyChanged();
        }
    }

    public IRelayCommand LoginCommand { get; }
	public IRelayCommand RegisterCommand { get; }
    public LoginViewModel()
    {
		LoginCommand = new RelayCommand(LoginExecuteCommand);
        RegisterCommand = new RelayCommand(RegisterExecuteCommand);
    }
    private void LoginExecuteCommand()
    {
        if (LoginUsername == String.Empty || LoginPassword == String.Empty)
        {
            MessageBox.Show("each user needs to have a username and a password!");
            return;
        }

        UserManager.LogIn(LoginUsername, LoginPassword);
    }
    private void RegisterExecuteCommand()
    {
        if (RegisterUsername == String.Empty || RegisterPassword == String.Empty)
        {
            MessageBox.Show("Each user needs to have a username and a password!");
            return;
        }

        UserManager.Register(RegisterUsername, RegisterPassword);
    }

    

}
