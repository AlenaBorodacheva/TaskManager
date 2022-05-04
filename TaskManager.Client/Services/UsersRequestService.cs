﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class UsersRequestService : CommonRequestService
{
    private const string _usersControllerUrl = HOST + "users";

    public AuthToken GetToken(string userName, string password)
    {
        string url = HOST + "account/token";
        string resultStr = GetDataByUrl(HttpMethod.Post, url, null, userName, password);
        AuthToken token = JsonConvert.DeserializeObject<AuthToken>(resultStr);
        return token;
    }

    public HttpStatusCode CreateUser(AuthToken token, UserModel user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        return SendDataByUrl(HttpMethod.Post, _usersControllerUrl, token, userJson);
    }

    public List<UserModel> GetAllUsers(AuthToken token)
    {
        string responce = GetDataByUrl(HttpMethod.Get, _usersControllerUrl, token);
        return JsonConvert.DeserializeObject<List<UserModel>>(responce);
    }

    public HttpStatusCode DeleteUser(AuthToken token, int userId)
    {
       return DeleteDataByUrl(_usersControllerUrl + $"/{userId}", token);
    }

    public HttpStatusCode CreateMultipleUsers(AuthToken token, List<UserModel> users)
    {
        string userJson = JsonConvert.SerializeObject(users);
        return SendDataByUrl(HttpMethod.Post, _usersControllerUrl + "/all", token, userJson);
    }

    public HttpStatusCode UpdateUser(AuthToken token, UserModel user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        return SendDataByUrl(HttpMethod.Patch, _usersControllerUrl + $"/{user.Id}", token, userJson);
    }
}