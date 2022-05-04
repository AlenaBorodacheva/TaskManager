using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class ProjectsRequestService : CommonRequestService
{
    private const string _projectsControllerUrl = HOST + "projects";

    public List<ProjectModel> GetAllProjects(AuthToken token)
    {
        string responce = GetDataByUrl(HttpMethod.Get, _projectsControllerUrl, token);
        return JsonConvert.DeserializeObject<List<ProjectModel>>(responce);
    }

    public ProjectModel GetProjectById(AuthToken token, int projectId)
    {
        var result = GetDataByUrl(HttpMethod.Get, _projectsControllerUrl + $"/{projectId}", token);
        return JsonConvert.DeserializeObject<ProjectModel>(result);
    }

    public HttpStatusCode CreateProject(AuthToken token, ProjectModel project)
    {
        string projectJson = JsonConvert.SerializeObject(project);
        return SendDataByUrl(HttpMethod.Post, _projectsControllerUrl, token, projectJson);
    }

    public HttpStatusCode UpdateProject(AuthToken token, ProjectModel project)
    {
        string projectJson = JsonConvert.SerializeObject(project);
        return SendDataByUrl(HttpMethod.Patch, _projectsControllerUrl + $"/{project.Id}", token, projectJson);
    }

    public HttpStatusCode DeleteProject(AuthToken token, int projectId)
    {
        return DeleteDataByUrl(_projectsControllerUrl + $"/{projectId}", token);
    }

    public HttpStatusCode AddUsersToProject(AuthToken token, int projectId, List<int> usersIds)
    {
        string usersIdsJson = JsonConvert.SerializeObject(usersIds);
        return SendDataByUrl(HttpMethod.Patch, _projectsControllerUrl + $"/{projectId}/users", token, usersIdsJson);
    }

    public HttpStatusCode RemoveUsersFromProject(AuthToken token, int projectId, List<int> usersIds)
    {
        string usersIdsJson = JsonConvert.SerializeObject(usersIds);
        return SendDataByUrl(HttpMethod.Patch, _projectsControllerUrl + $"/{projectId}/users/remove", token, usersIdsJson);
    }
}