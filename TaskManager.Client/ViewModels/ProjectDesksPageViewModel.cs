using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class ProjectDesksPageViewModel  : BindableBase
{
    #region PROPERTIES

    private CommonViewService _viewService;
    private AuthToken _token;
    private UsersRequestService _usersRequestService;
    private ProjectModel _project;
    private DesksRequestService _desksRequestService;
    private DesksViewService _desksViewService;
    private MainWindowViewModel _mainWindowVM;

    private List<ModelClient<DeskModel>> _projectDesks = new List<ModelClient<DeskModel>>();

    public List<ModelClient<DeskModel>> ProjectDesks
    {
        get => _projectDesks;
        set
        {
            _projectDesks = value;
            RaisePropertyChanged(nameof(ProjectDesks));
        }
    }

    private ClientAction _typeActionWithDesk;

    public ClientAction TypeActionWithDesk
    {
        get => _typeActionWithDesk;
        set
        {
            _typeActionWithDesk = value;
            RaisePropertyChanged(nameof(TypeActionWithDesk));
        }
    }

    private ModelClient<DeskModel> _selectedDesk;

    public ModelClient<DeskModel> SelectedDesk
    {
        get => _selectedDesk;
        set
        {
            _selectedDesk = value;
            RaisePropertyChanged(nameof(SelectedDesk));
        }
    }

    public UserModel CurrentUser
    {
        get => _usersRequestService.GetCurrentUser(_token);
    }

    private ObservableCollection<ColumnBindingHelp> _columnsForNewDesk = new ObservableCollection<ColumnBindingHelp>()
    {
        new ColumnBindingHelp("New"),
        new ColumnBindingHelp("In progress"),
        new ColumnBindingHelp("In review"),
        new ColumnBindingHelp("Completed")
    };

    public ObservableCollection<ColumnBindingHelp> ColumnsForNewDesk
    {
        get => _columnsForNewDesk;
        set
        {
            _columnsForNewDesk = value;
            RaisePropertyChanged(nameof(ColumnsForNewDesk));
        }
    }

    #endregion

    #region COMMANDS

    public DelegateCommand OpenNewDeskCommand { get; private set; }
    public DelegateCommand<object> OpenUpdateDeskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
    public DelegateCommand DeleteDeskCommand { get; private set; }
    public DelegateCommand SelectPhotoForDeskCommand { get; private set; }
    public DelegateCommand AddNewColumnItemCommand { get; private set; }
    public DelegateCommand<object> RemoveColumnItemCommand { get; private set; }
    public DelegateCommand<object> OpenDeskTasksPageCommand { get; private set; }

    #endregion

    public ProjectDesksPageViewModel(AuthToken token, ProjectModel project, MainWindowViewModel MainWindowVM)
    {
        _token = token;
        _project = project;
        _mainWindowVM = MainWindowVM;

        _usersRequestService = new UsersRequestService();
        _viewService = new CommonViewService();
        _desksRequestService = new DesksRequestService();
        _desksViewService = new DesksViewService(_token, _desksRequestService);

        UpdatePage();

        OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
        OpenUpdateDeskCommand = new DelegateCommand<object>(OpenUpdateDesk);
        CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDesk);
        DeleteDeskCommand = new DelegateCommand(DeleteDesk);
        SelectPhotoForDeskCommand = new DelegateCommand(SelectPhotoForDesk);
        AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
        RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);
        OpenDeskTasksPageCommand = new DelegateCommand<object>(OpenDeskTasksPage);
    }
    
    private void OpenNewDesk()
    {
        SelectedDesk = new ModelClient<DeskModel>(new DeskModel());
        TypeActionWithDesk = ClientAction.Create;
        var wnd = new CreateOrUpdateDeskWindow();
        _viewService.OpenWindow(wnd, this);
    }

    private void OpenUpdateDesk(object deskId)
    {
        SelectedDesk = _desksViewService.GetDeskClientById(deskId);
        if (CurrentUser.Id != SelectedDesk.Model.AdminId)
        {
            _viewService.ShowMessage("You are not admin!");
            return;
        }
        TypeActionWithDesk = ClientAction.Update;
        ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelp>(SelectedDesk.Model.Columns.Select(c => new ColumnBindingHelp(c)));
        _desksViewService.OpenViewDeskInfo(deskId, this);
    }

    public void CreateOrUpdateDesk()
    {
        if (TypeActionWithDesk == ClientAction.Create)
        {
            CreateDesk();
        }

        if (TypeActionWithDesk == ClientAction.Update)
        {
            UpdateDesk();
        }

        UpdatePage();
    }

    private void CreateDesk()
    {
        SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
        SelectedDesk.Model.ProjectId = _project.Id;

        var resultAction = _desksRequestService.CreateDesk(_token, SelectedDesk.Model);
        _viewService.ShowActionResult(resultAction, "\nNew Desk is created");
    }

    private void UpdateDesk()
    {
        SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
        _desksViewService.UpdateDesk(SelectedDesk.Model);
    }

    private void DeleteDesk()
    {
        _desksViewService.DeleteDesk(SelectedDesk.Model.Id);
        UpdatePage();
        _viewService.CurrentOpenWindow?.Close();
    }

    private void UpdatePage()
    {
        SelectedDesk = null;
        ProjectDesks = _desksViewService.GetDesks(_project.Id);
        _viewService.CurrentOpenWindow?.Close();
    }

    private void SelectPhotoForDesk()
    {
        SelectedDesk = _desksViewService.SelectPhotoForDesk(SelectedDesk);
    }

    private void AddNewColumnItem()
    {
        ColumnsForNewDesk.Add(new ColumnBindingHelp("Column"));
    }

    private void RemoveColumnItem(object item)
    {
        var itemToRemove = item as ColumnBindingHelp;
        ColumnsForNewDesk.Remove(itemToRemove);
    }

    private void OpenDeskTasksPage(object deskId)
    {
        SelectedDesk = _desksViewService.GetDeskClientById(deskId);
        var page = new DeskTasksPage();
        var context = new DeskTasksPageViewModel(_token, SelectedDesk.Model, page);
        _mainWindowVM.OpenPage(page, $"Tasks of {SelectedDesk.Model.Name}", context);
    }
}