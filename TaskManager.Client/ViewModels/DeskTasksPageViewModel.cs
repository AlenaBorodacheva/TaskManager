using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class DeskTasksPageViewModel : BindableBase
{
    private AuthToken _token;
    private DeskModel _desk;
    private UsersRequestService _usersRequestService;
    private TasksRequestService _tasksRequestService;
    private CommonViewService _viewService;    

    #region PROPERTIES

    private Dictionary<string, List<TaskClient>> _tasksByColumns = new Dictionary<string, List<TaskClient>>();

    public Dictionary<string, List<TaskClient>> TasksByColumns
    {
        get => _tasksByColumns;
        set
        {
            _tasksByColumns = value;
            RaisePropertyChanged(nameof(TasksByColumns));
        }
    }

    #endregion

    public DeskTasksPageViewModel(AuthToken token, DeskModel desk)
    {
        _token = token;
        _desk = desk;        

        _usersRequestService = new UsersRequestService();
        _tasksRequestService = new TasksRequestService();
        _viewService = new CommonViewService();

        TasksByColumns = GetTasksByColumns(_desk.Id);
    }

    #region METHODS

    private Dictionary<string, List<TaskClient>> GetTasksByColumns(int deskId)
    {
        var tasksByColumns = new Dictionary<string, List<TaskClient>>();
        var allTasks = _tasksRequestService.GetTasksByDesk(_token, deskId);
        foreach(var column in _desk.Columns)
        {
            tasksByColumns.Add(column, allTasks.Where(t => t.Column == column).Select(t => new TaskClient(t)).ToList());
        }
        return tasksByColumns;
    }

    #endregion
}

