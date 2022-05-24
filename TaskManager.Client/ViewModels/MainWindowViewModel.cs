using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private CommonViewService _viewService;

    #region COMMANDS

    public DelegateCommand OpenMyInfoPageCommand { get; private set; }
    public DelegateCommand OpenProjectsPageCommand { get; private set; }
    public DelegateCommand OpenDesksPageCommand { get; private set; }
    public DelegateCommand OpenTasksPageCommand { get; private set; }
    public DelegateCommand LogoutCommand { get; private set; }
    public DelegateCommand OpenUsersManagementCommand { get; private set; }

    #endregion

    #region PROPERTIES

    private readonly string _logoutBtnName = "Logout";
    private readonly string _userInfoBtnName = "My info";
    private readonly string _userTasksBtnName = "My tasks";
    private readonly string _userDesksBtnName = "My desks";
    private readonly string _userProjectsBtnName = "My projects";
    private readonly string _manageUsersBtnName = "Users";

    private Window _currentWindow;

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

    private string _selectedPageName;

    public string SelectedPageName
    {
        get => _selectedPageName;
        set
        {
            _selectedPageName = value;
            RaisePropertyChanged(nameof(SelectedPageName));
        }
    }

    private Page _selectedPage;

    public Page SelectedPage
    {
        get => _selectedPage;
        set
        {
            _selectedPage = value;
            RaisePropertyChanged(nameof(SelectedPage));
        }
    }

    #endregion

    public MainWindowViewModel(AuthToken token, UserModel currentUser, Window? currentWindow = null)
    {
        _viewService = new CommonViewService();
        AuthToken = token;
        CurrentUser = currentUser;
        _currentWindow = currentWindow;

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

        OpenMyInfoPage();
    }
    
    #region METHODS

    private void OpenMyInfoPage()
    {
        var page = new UserInfoPage();
        OpenPage(page, _userInfoBtnName, this);
    }

    private void OpenDesksPage()
    {
        SelectedPageName = _userDesksBtnName;
        _viewService.ShowMessage(_userDesksBtnName);
    }

    private void OpenProjectsPage()
    {
        var page = new ProjectsPage();
        OpenPage(page, _userProjectsBtnName, new ProjectsPageViewModel(AuthToken, this));
    }

    private void OpenTasksPage()
    {
        var page = new UserTasksPage();
        OpenPage(page, _userTasksBtnName, new UserTasksPageViewModel(_authToken));
    }

    private void Logout()
    {
        var question = MessageBox.Show("Are you sure?", "Logout", MessageBoxButton.YesNo);
        if (question == MessageBoxResult.Yes && _currentWindow != null)
        {
            Login login = new Login();
            login.Show();
            _currentWindow.Close();
        }
    }

    private void OpenUsersManagement()
    {
        SelectedPageName = _manageUsersBtnName;
        _viewService.ShowMessage(_manageUsersBtnName);
    }

    #endregion
    
    public void OpenPage(Page page, string pageName, BindableBase viewModel)
    {
        SelectedPageName = pageName;
        SelectedPage = page;
        SelectedPage.DataContext = viewModel;
    }
}