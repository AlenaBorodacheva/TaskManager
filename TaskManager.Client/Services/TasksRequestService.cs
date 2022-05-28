using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class TasksRequestService : CommonRequestService
{
    private const string _tasksControllerUrl = HOST + "tasks";

    public List<TaskModel> GetAllTasks(AuthToken token)
    {
        string responce = GetDataByUrl(HttpMethod.Get, _tasksControllerUrl + "/user", token);
        return JsonConvert.DeserializeObject<List<TaskModel>>(responce);
    }

    public TaskModel GetTaskById(AuthToken token, int taskId)
    {
        var result = GetDataByUrl(HttpMethod.Get, _tasksControllerUrl + $"/{taskId}", token);
        return JsonConvert.DeserializeObject<TaskModel>(result);
    }

    public List<TaskModel> GetTasksByDesk(AuthToken token, int deskId)
    {
        var parameters = new Dictionary<string, string>();
        parameters.Add("deskId", deskId.ToString());

        string responce = GetDataByUrl(HttpMethod.Get, _tasksControllerUrl, token, null, null, parameters);
        return JsonConvert.DeserializeObject<List<TaskModel>>(responce);
    }

    public HttpStatusCode CreateTask(AuthToken token, TaskModel task)
    {
        string taskJson = JsonConvert.SerializeObject(task);
        return SendDataByUrl(HttpMethod.Post, _tasksControllerUrl, token, taskJson);
    }

    public HttpStatusCode UpdateTask(AuthToken token, TaskModel task)
    {
        string taskJson = JsonConvert.SerializeObject(task);
        return SendDataByUrl(HttpMethod.Patch, _tasksControllerUrl + $"/{task.Id}", token, taskJson);
    }

    public HttpStatusCode DeleteTask(AuthToken token, int taskId)
    {
        return DeleteDataByUrl(_tasksControllerUrl + $"/{taskId}", token);
    }
}