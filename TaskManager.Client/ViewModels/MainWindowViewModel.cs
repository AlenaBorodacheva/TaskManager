using System.Collections.Generic;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class MainWindowViewModel : BindableBase
{
    #region COMMANDS

    public DelegateCommand OpenMyInfoPageCommand;
    public DelegateCommand OpenProjectsPageCommand;
    public DelegateCommand OpenDesksPageCommand;
    public DelegateCommand OpenTasksPageCommand;
    public DelegateCommand LogoutCommand;
    public DelegateCommand OpenUsersManagementCommand;

    #endregion

    #region PROPERTIES

    private readonly string _logoutBtnName = "Logout";
    private readonly string _userInfoBtnName = "My info";
    private readonly string _userTasksBtnName = "My tasks";
    private readonly string _userDesksBtnName = "My desks";
    private readonly string _userProjectsBtnName = "My projects";
    private readonly string _manageUsersBtnName = "Users";

    private Dictionary<string, DelegateCommand> _navigationButtons = new Dictionary<string, DelegateCommand>();

    public Dictionary<string, DelegateCommand> NavigationButtons
    {
        get => _navigationButtons;
        set
        {
            _navigationButtons = value;
            RaisePropertyChanged(nameof(NavigationButtons));
        }
    }

    private AuthToken _authToken;

    public AuthToken AuthToken
    {
        get => _authToken;
        private set
        {
            _authToken = value;
            RaisePropertyChanged(nameof(AuthToken));
        }
    }

    private UserModel _currentUser;

    public UserModel CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            RaisePropertyChanged(nameof(CurrentUser));
        }
    }

    #endregion

    public MainWindowViewModel(AuthToken token, UserModel currentUser)
    {
        AuthToken = token;
        CurrentUser = currentUser;

        OpenMyInfoPageCommand = new DelegateCommand(OpenMyInfoPage);
        NavigationButtons.Add(_userInfoBtnName, OpenMyInfoPageCommand);

        OpenProjectsPageCommand = new DelegateCommand(OpenProjectsPage);
        NavigationButtons.Add(_userProjectsBtnName, OpenProjectsPageCommand);

        OpenDesksPageCommand = new DelegateCommand(OpenDesksPage);
        NavigationButtons.Add(_userDesksBtnName, OpenDesksPageCommand);

        OpenTasksPageCommand = new DelegateCommand(OpenTasksPage);
        NavigationButtons.Add(_userTasksBtnName, OpenTasksPageCommand);

        if (CurrentUser.Status == UserStatus.Admin)
        {
            OpenUsersManagementCommand = new DelegateCommand(OpenUsersManagement);
            NavigationButtons.Add(_manageUsersBtnName, OpenUsersManagementCommand);
        }

        LogoutCommand = new DelegateCommand(Logout);
        NavigationButtons.Add(_logoutBtnName, LogoutCommand);
    }

    

    #region METHODS

    private void OpenMyInfoPage()
    {
        ShowMessage(_userInfoBtnName);
    }

    private void OpenDesksPage()
    {
        ShowMessage(_userDesksBtnName);
    }

    private void OpenProjectsPage()
    {
        ShowMessage(_userProjectsBtnName);
    }

    private void OpenTasksPage()
    {
        ShowMessage(_userTasksBtnName);
    }

    private void Logout()
    {
        ShowMessage(_logoutBtnName);
    }

    private void OpenUsersManagement()
    {
        ShowMessage(_manageUsersBtnName);
    }

    #endregion

    private void ShowMessage(string message)
    {
        MessageBox.Show(message);
    }
}