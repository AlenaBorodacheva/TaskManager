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

    public ProjectDesksPageViewModel(AuthToken token, ProjectModel project)
    {
        _token = token;
        _project = project;
        _usersRequestService = new UsersRequestService();
        _viewService = new CommonViewService();
        _desksRequestService = new DesksRequestService();
        UpdatePage();
        OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
        OpenUpdateDeskCommand = new DelegateCommand<object>(OpenUpdateDesk);
        CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDesk);
        DeleteDeskCommand = new DelegateCommand(DeleteDesk);
        SelectPhotoForDeskCommand = new DelegateCommand(SelectPhotoForDesk);
        AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
        RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);
    }

    #region COMMANDS

    public DelegateCommand OpenNewDeskCommand { get; private set; }
    public DelegateCommand<object> OpenUpdateDeskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
    public DelegateCommand DeleteDeskCommand { get; private set; }
    public DelegateCommand SelectPhotoForDeskCommand { get; private set; }
    public DelegateCommand AddNewColumnItemCommand { get; private set; }
    public DelegateCommand<object> RemoveColumnItemCommand { get; private set; }

    #endregion

    private List<ModelClient<DeskModel>> GetDesks()
    {
        var result = new List<ModelClient<DeskModel>>();
        var desks = _desksRequestService.GetDesksByProject(_token, _project.Id);
        if (desks != null)
        {
            result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();
        }

        return result;
    }

    private void OpenNewDesk()
    {
        SelectedDesk = new ModelClient<DeskModel>(new DeskModel());
        TypeActionWithDesk = ClientAction.Create;
        var wnd = new CreateOrUpdateDeskWindow();
        _viewService.OpenWindow(wnd, this);
    }

    private void OpenUpdateDesk(object DeskId)
    {
        SelectedDesk = GetDeskClientById(DeskId);

        TypeActionWithDesk = ClientAction.Update;
        var wnd = new CreateOrUpdateDeskWindow();
        _viewService.OpenWindow(wnd, this);
    }

    private ModelClient<DeskModel> GetDeskClientById(object deskId)
    {
        try
        {
            int id = (int)deskId;
            DeskModel desk = _desksRequestService.GetDeskById(_token, id);
            return new ModelClient<DeskModel>(desk);
        }
        catch (FormatException)
        {
            return new ModelClient<DeskModel>(null);
        }
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
        var resultAction = _desksRequestService.UpdateDesk(_token, SelectedDesk.Model);
        _viewService.ShowActionResult(resultAction, "\nDesk is updated");
    }

    private void DeleteDesk()
    {
        var resultAction = _desksRequestService.DeleteDesk(_token, SelectedDesk.Model.Id);
        _viewService.ShowActionResult(resultAction, "\nDesk is deleted");

        UpdatePage();
        _viewService.CurrentOpenWindow?.Close();
    }

    private void UpdatePage()
    {
        SelectedDesk = null;
        ProjectDesks = GetDesks();
        _viewService.CurrentOpenWindow?.Close();
    }

    private void SelectPhotoForDesk()
    {
        _viewService.SetPhotoForObject(SelectedDesk.Model);
        SelectedDesk = new ModelClient<DeskModel>(SelectedDesk.Model);
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
}