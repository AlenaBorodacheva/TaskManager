using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;

namespace TaskManager.Client.ViewModels;

public class UserTasksPageViewModel : BindableBase
{
    private AuthToken _token;
    private TasksRequestService _tasksRequestService;
    private UsersRequestService _usersRequestService;

    public List<TaskClient> AllTasks
    {
        get => _tasksRequestService.GetAllTasks(_token).Select(
            task =>
            {
                var taskClient = new TaskClient(task);
                int? creatorId = task.CreatorId;
                int? executorId = task.ExecutorId;

                if(creatorId != null)
                    taskClient.Creator = _usersRequestService.GetUserById(_token, creatorId);
                if(executorId != null)
                    taskClient.Executor = _usersRequestService.GetUserById(_token, executorId);

                return taskClient;
            }).ToList();
    }

    public UserTasksPageViewModel(AuthToken token)
    {
        _token = token;
        _tasksRequestService = new TasksRequestService();
        _usersRequestService = new UsersRequestService();
    }
}