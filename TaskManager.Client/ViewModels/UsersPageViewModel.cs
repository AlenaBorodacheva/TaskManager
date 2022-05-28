using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class UsersPageViewModel : BindableBase
{
    private AuthToken _token;
    private UsersRequestService _usersRequestService;
    private CommonViewService _viewService;
    private ExcelService _excelService;

    #region COMMANDS

    public DelegateCommand<object> OpenUpdateUserCommand { get; private set; }
    public DelegateCommand OpenNewUserCommand { get; private set; }
    public DelegateCommand<object> DeleteUserCommand { get; private set; }
    public DelegateCommand CreateOrUpdateUserCommand { get; private set; }
    public DelegateCommand OpenSelectUsersFromExcelCommand { get; private set; }
    public DelegateCommand GetUsersFromExcelCommand { get; private set; }
    public DelegateCommand AddUsersFromExcelCommand { get; private set; }

    #endregion

    #region PROPERTIES

    private List<UserModel> _allUsers = new List<UserModel>();

    public List<UserModel> AllUsers
    {
        get => _allUsers;
        set
        {
            _allUsers = value;
            RaisePropertyChanged(nameof(AllUsers));
        }
    }

    private List<UserModel> _selectedUsersFromExcel = new List<UserModel>();

    public List<UserModel> SelectedUsersFromExcel
    {
        get => _selectedUsersFromExcel; 
        set 
        { 
            _selectedUsersFromExcel = value;
            RaisePropertyChanged(nameof(SelectedUsersFromExcel));
        }
    }

    private List<UserModel> _usersFromExcel;

    public List<UserModel> UsersFromExcel
    {
        get => _usersFromExcel;
        set
        {
            _usersFromExcel = value;
            RaisePropertyChanged(nameof(UsersFromExcel));
        }
    }

    private UserModel _selectedUser;

    public UserModel SelectedUser
    {
        get => _selectedUser; 
        set 
        { 
            _selectedUser = value;
            RaisePropertyChanged(nameof(SelectedUser));
        }
    }

    private ClientAction _typeActionWithUser;
    public ClientAction TypeActionWithUser
    {
        get => _typeActionWithUser;
        set
        {
            _typeActionWithUser = value;
            RaisePropertyChanged(nameof(TypeActionWithUser));
        }
    }

    private string _excelDialogFilterPattern = "Excel Files(.xls)|*.xls| Excel Files(.xlsx)|*.xlsx| Excel Files(*.xlsm)|*.xlsm";

    #endregion

    public UsersPageViewModel(AuthToken token)
    {
        _token = token;
        _usersRequestService = new UsersRequestService();   
        _viewService = new CommonViewService();
        _excelService = new ExcelService();

        OpenNewUserCommand = new DelegateCommand(OpenNewUser);
        DeleteUserCommand = new DelegateCommand<object>(DeleteUser);
        CreateOrUpdateUserCommand = new DelegateCommand(CreateOrUpdateUser);
        OpenUpdateUserCommand = new DelegateCommand<object>(OpenUpdateUser);
        OpenSelectUsersFromExcelCommand = new DelegateCommand(OpenSelectUsersFromExcel);
        GetUsersFromExcelCommand = new DelegateCommand(GetUsersFromExcel);
        AddUsersFromExcelCommand = new DelegateCommand(AddUsersFromExcel);
        AllUsers = _usersRequestService.GetAllUsers(_token);
    }

    #region METHODS

    private void OpenUpdateUser(object userObj)
    {
        if (userObj != null)
        {
            TypeActionWithUser = ClientAction.Update;
            SelectedUser = _usersRequestService.GetUserById(_token, ((UserModel)userObj).Id);
            var wnd = new CreateOrUpdateUserWindow();
            _viewService.OpenWindow(wnd, this);
        }            
    }

    private void OpenNewUser()
    {
        TypeActionWithUser = ClientAction.Create;
        SelectedUser = new UserModel();
        var wnd = new CreateOrUpdateUserWindow();
        _viewService.OpenWindow(wnd, this);
    }

    private void DeleteUser(object userObj)
    {
        if(userObj != null)
        {
            int userId = ((UserModel)userObj).Id;
            _usersRequestService.DeleteUser(_token, userId);
            UpdatePage();
        }        
    }

    private void CreateOrUpdateUser()
    {
        if (TypeActionWithUser == ClientAction.Create)
        {
            _usersRequestService.CreateUser(_token, SelectedUser);
        }

        if (TypeActionWithUser == ClientAction.Update)
        {
            _usersRequestService.UpdateUser(_token, SelectedUser);
        }

        UpdatePage();
    }

    private void OpenSelectUsersFromExcel()
    {
        var wnd = new UsersFromExcelWindow();
        _viewService.OpenWindow(wnd, this);
    }

    private void GetUsersFromExcel()
    {
        string path = _viewService.GetFileFromDialog(_excelDialogFilterPattern);
        if(!string.IsNullOrEmpty(path))
            UsersFromExcel = _excelService.GetAllUsersFromExcel(path);
    }

    private void AddUsersFromExcel()
    {
        if(SelectedUsersFromExcel != null && SelectedUsersFromExcel.Count > 0)
        {
            var result = _usersRequestService.CreateMultipleUsers(_token, SelectedUsersFromExcel);
            _viewService.ShowActionResult(result, "All users are created");
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        _viewService.CurrentOpenWindow?.Close();
        SelectedUser = null;
        SelectedUsersFromExcel = new List<UserModel>();
        AllUsers = _usersRequestService.GetAllUsers(_token);
    }

    #endregion
}
