using System.Windows;
using System.Windows.Controls;
using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class LoginViewModel : BindableBase
{
    private UsersRequestService _usersRequestService;

    #region COMMANDS

    public DelegateCommand<object> GetUserFromDbCommand { get; private set; }

    public string UserLogin { get; set; }

    public string UserPassword { get; private set; }

    private UserModel _currentUser;

    public UserModel CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            RaisePropertyChanged(nameof(CurrentUser));
        }
    }

    private AuthToken _authToken;

    public AuthToken AuthToken
    {
        get => _authToken;
        set
        {
            _authToken = value;
            RaisePropertyChanged(nameof(CurrentUser));
        }
    }

    public LoginViewModel()
    {
        _usersRequestService = new UsersRequestService();
        GetUserFromDbCommand = new DelegateCommand<object>(GetUserFromDb);
    }

    #endregion

    #region METHODS

    private void GetUserFromDb(object parameter)
    {
        var passBox = parameter as PasswordBox;
        UserPassword = passBox.Password;
        AuthToken = _usersRequestService.GetToken(UserLogin, UserPassword);
        if (AuthToken != null)
        {
            CurrentUser = _usersRequestService.GetCurrentUser(AuthToken);
            if (CurrentUser != null)
            {
                MessageBox.Show(CurrentUser.FirstName);
            }
        }
    }
    
    #endregion
}