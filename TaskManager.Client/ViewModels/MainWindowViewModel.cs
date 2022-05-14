using System.Collections.Generic;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;

namespace TaskManager.Client.ViewModels;

public class MainWindowViewModel : BindableBase
{
    #region COMMANDS

    public DelegateCommand OpenMyInfoPageCommand;
    public DelegateCommand OpenProjectsPageCommand;
    public DelegateCommand OpenDesksPageCommand;
    public DelegateCommand OpenTasksPageCommand;
    public DelegateCommand LogoutCommand;

    #endregion

    #region PROPERTIES

    private readonly string _logoutBtnName = "Logout";
    private readonly string _userInfoBtnName = "My info";
    private readonly string _userTasksBtnName = "My tasks";
    private readonly string _userDesksBtnName = "My desks";
    private readonly string _userProjectsBtnName = "My projects";

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

    #endregion

    public MainWindowViewModel()
    {
        OpenMyInfoPageCommand = new DelegateCommand(OpenMyInfoPage);
        NavigationButtons.Add(_userInfoBtnName, OpenMyInfoPageCommand);

        OpenProjectsPageCommand = new DelegateCommand(OpenProjectsPage);
        NavigationButtons.Add(_userProjectsBtnName, OpenProjectsPageCommand);

        OpenDesksPageCommand = new DelegateCommand(OpenDesksPage);
        NavigationButtons.Add(_userDesksBtnName, OpenDesksPageCommand);

        OpenTasksPageCommand = new DelegateCommand(OpenTasksPage);
        NavigationButtons.Add(_userTasksBtnName, OpenTasksPageCommand);

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

    #endregion

    private void ShowMessage(string message)
    {
        MessageBox.Show(message);
    }
}