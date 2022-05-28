using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class UserDesksPageViewModel : BindableBase
{
    private AuthToken _token;
    private DesksRequestService _desksRequestService;    
    private DesksViewService _desksViewService;

    #region COMMANDS

    public DelegateCommand OpenEditDeskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
    public DelegateCommand DeleteDeskCommand { get; private set; }
    public DelegateCommand SelectPhotoForDeskCommand { get; private set; }
    public DelegateCommand AddNewColumnItemCommand { get; private set; }
    public DelegateCommand<object> RemoveColumnItemCommand { get; private set; }

    #endregion

    #region PROPERTIES

    private List<ModelClient<DeskModel>> _allDesks = new List<ModelClient<DeskModel>>();
    public List<ModelClient<DeskModel>> AllDesks
    {
        get => _allDesks;
        set
        {
            _allDesks = value;
            RaisePropertyChanged(nameof(AllDesks));
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

    private Dictionary<string, DelegateCommand> _contextMenuCommands = new Dictionary<string, DelegateCommand>();

    public Dictionary<string, DelegateCommand> ContextMenuCommands
    {
        get => _contextMenuCommands;
        set
        {
            _contextMenuCommands = value;
            RaisePropertyChanged(nameof(ContextMenuCommands));
        }
    }

    private ObservableCollection<ColumnBindingHelp> _columnsForNewDesk;

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

    public UserDesksPageViewModel(AuthToken token)
    {
        _token = token;
        _desksRequestService = new DesksRequestService();
        _desksViewService = new DesksViewService(_token, _desksRequestService);

        OpenEditDeskCommand = new DelegateCommand(OpenUpdateDesk);
        CreateOrUpdateDeskCommand = new DelegateCommand(UpdateDesk);
        DeleteDeskCommand = new DelegateCommand(DeleteDesk);
        SelectPhotoForDeskCommand = new DelegateCommand(SelectPhotoForDesk);
        AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
        RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);

        ContextMenuCommands.Add("Edit", OpenEditDeskCommand);
        ContextMenuCommands.Add("Delete", DeleteDeskCommand);

        UpdatePage();
    }

    #region METHODS    

    private void OpenUpdateDesk()
    {
        SelectedDesk = _desksViewService.GetDeskClientById(SelectedDesk.Model.Id);
        ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelp>(SelectedDesk.Model.Columns.Select(c => new ColumnBindingHelp(c)));
        _desksViewService.OpenViewDeskInfo(SelectedDesk.Model.Id, this);
        _desksViewService.ViewService.CurrentOpenWindow?.Close();
    }

    private void UpdateDesk()
    {
        SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
        _desksViewService.UpdateDesk(SelectedDesk.Model);
        UpdatePage();
    }

    private void SelectPhotoForDesk()
    {
        SelectedDesk = _desksViewService.SelectPhotoForDesk(SelectedDesk);
    }

    private void DeleteDesk()
    {
        _desksViewService.DeleteDesk(SelectedDesk.Model.Id);
        UpdatePage();
    }

    private void UpdatePage()
    {
        SelectedDesk = null;
        AllDesks = _desksViewService.GetAllDesks();
        _desksViewService.ViewService.CurrentOpenWindow?.Close();
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

    #endregion
}