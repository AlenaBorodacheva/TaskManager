using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class LoginViewModel : BindableBase
{
    private UsersRequestService _usersRequestService;

    #region COMMANDS

    public DelegateCommand<object> GetUserFromDbCommand { get; private set; }
    public DelegateCommand<object> LoginFromCacheCommand { get; private set; }

    #endregion

    #region PROPERTIES

    private string _cachePath = Path.GetTempPath() + "UserTaskManager.txt";

    private Window _currentWindow;
    public string UserLogin { get; set; }
    public string UserPassword { get; private set; }

    private UserCache _currentUserCache;

    public UserCache CurrentUserCache
    {
        get => _currentUserCache;
        set
        {
            _currentUserCache = value;
            RaisePropertyChanged(nameof(CurrentUserCache));
        }
    }

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

    #endregion

    public LoginViewModel()
    {
        _usersRequestService = new UsersRequestService();
        CurrentUserCache = GetUserCache();
        GetUserFromDbCommand = new DelegateCommand<object>(GetUserFromDb);
        LoginFromCacheCommand = new DelegateCommand<object>(LoginFromCache);
    }

    #region METHODS

    private void GetUserFromDb(object parameter)
    {
        var passBox = parameter as PasswordBox;
        _currentWindow = Window.GetWindow(passBox);

        bool isNewUser = false;
        if (UserLogin != CurrentUserCache?.Login ||
            UserPassword != CurrentUserCache?.Password)
        {
            isNewUser = true;
        }

        UserPassword = passBox.Password;
        AuthToken = _usersRequestService.GetToken(UserLogin, UserPassword);
        if (AuthToken == null)
        {
            return;
        }

        CurrentUser = _usersRequestService.GetCurrentUser(AuthToken);
        if (CurrentUser != null)
        {
            if (isNewUser)
            {
                var saveUserCacheMessage = MessageBox.Show("Хотите сохранить логин и пароль?", "Сохранение данных", MessageBoxButton.YesNo);
                if (saveUserCacheMessage == MessageBoxResult.Yes)
                {
                    UserCache newUserCache = new UserCache()
                    {
                        Login = UserLogin,
                        Password = UserPassword
                    };
                    CreateUserCache(newUserCache);
                }
            }

            OpenMainWindow();
        }
    }

    private void CreateUserCache(UserCache userCache)
    {
        string jsonUserCache = JsonConvert.SerializeObject(userCache);
        using (StreamWriter sw = new StreamWriter(_cachePath, false, Encoding.Default))
        {
            sw.Write(jsonUserCache);
            MessageBox.Show("Успех");
        }
    }

    private UserCache GetUserCache()
    {
        bool isCacheExist = File.Exists(_cachePath);

        if(isCacheExist && File.ReadAllText(_cachePath).Length > 0)
            return JsonConvert.DeserializeObject<UserCache>(File.ReadAllText(_cachePath));

        return null;
    }

    private void LoginFromCache(object wnd)
    {
        _currentWindow = wnd as Window;
        UserLogin = CurrentUserCache.Login;
        UserPassword = CurrentUserCache.Password;
        AuthToken = _usersRequestService.GetToken(UserLogin, UserPassword);

        CurrentUser = _usersRequestService.GetCurrentUser(AuthToken);
        if (CurrentUser != null)
        {
            OpenMainWindow();
        }
    }

    private void OpenMainWindow()
    {
        MainWindow window = new MainWindow();
        window.Show();
        _currentWindow.Close();
    }

    #endregion
}