using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class DesksRequestService : CommonRequestService
{
    private const string _desksControllerUrl = HOST + "desks";

    public List<DeskModel> GetAllDesks(AuthToken token)
    {
        string responce = GetDataByUrl(HttpMethod.Get, _desksControllerUrl, token);
        return JsonConvert.DeserializeObject<List<DeskModel>>(responce);
    }

    public DeskModel GetDeskById(AuthToken token, int deskId)
    {
        var result = GetDataByUrl(HttpMethod.Get, _desksControllerUrl + $"/{deskId}", token);
        return JsonConvert.DeserializeObject<DeskModel>(result);
    }

    public HttpStatusCode DeleteDesk(AuthToken token, int deskId)
    {
        return DeleteDataByUrl(_desksControllerUrl + $"/{deskId}", token);
    }

    public List<DeskModel> GetDesksByProject(AuthToken token, int projectId)
    {
        var parameters = new Dictionary<string, string>();
        parameters.Add("projectId", projectId.ToString());

        string responce = GetDataByUrl(HttpMethod.Get, _desksControllerUrl + "/project", token, null, null, parameters);
        return JsonConvert.DeserializeObject<List<DeskModel>>(responce);
    }

    public HttpStatusCode CreateDesk(AuthToken token, DeskModel desk)
    {
        string deskJson = JsonConvert.SerializeObject(desk);
        return SendDataByUrl(HttpMethod.Post, _desksControllerUrl, token, deskJson);
    }

    public HttpStatusCode UpdateDesk(AuthToken token, DeskModel desk)
    {
        string deskJson = JsonConvert.SerializeObject(desk);
        return SendDataByUrl(HttpMethod.Patch, _desksControllerUrl + $"/{desk.Id}", token, deskJson);
    }
}