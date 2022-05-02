﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class UsersRequestService
{
    private const string HOST = "http://localhost:52012/api/";
    private const string _userController = HOST + "users";

    private string GetDataByUrl(string url, string userName = null, string password = null)
    {
        string result = string.Empty;
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        request.Method = "POST";
        if (userName != null && password != null)
        {
            string encoded =
                Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
        }

        HttpWebResponse responce = (HttpWebResponse)request.GetResponse();
        using (StreamReader reader = new StreamReader(responce.GetResponseStream(), Encoding.UTF8))
        {
           result = reader.ReadToEnd();
        }
        
        return result;
    }

    private HttpStatusCode SendDataByUrl(HttpMethod method, string url, AuthToken token, string data = null)
    {
        HttpResponseMessage result = new HttpResponseMessage();
        HttpClient client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
        
        var content = new StringContent(data, Encoding.UTF8, "application/json");
        if (method == HttpMethod.Post)
        {
            result = client.PostAsync(url, content).Result;
        }

        if (method == HttpMethod.Patch)
        {
            result = client.PatchAsync(url, content).Result;
        }
        
        return result.StatusCode;
    }

    private HttpStatusCode DeleteDataByUrl(string url, AuthToken token)
    {
        HttpResponseMessage result = new HttpResponseMessage();
        HttpClient client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
        result = client.DeleteAsync(url).Result;

        return result.StatusCode;
    }

    public AuthToken GetToken(string userName, string password)
    {
        string url = HOST + "account/token";
        string resultStr = GetDataByUrl(url, userName, password);
        AuthToken token = JsonConvert.DeserializeObject<AuthToken>(resultStr);
        return token;
    }

    public HttpStatusCode CreateUser(AuthToken token, UserModel user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        return SendDataByUrl(HttpMethod.Post, _userController, token, userJson);
    }

    public List<UserModel> GetAllUsers(AuthToken token)
    {
        string responce = GetDataByUrl(_userController);
        return JsonConvert.DeserializeObject<List<UserModel>>(responce);
    }

    public HttpStatusCode DeleteUser(AuthToken token, int userId)
    {
       return DeleteDataByUrl(_userController + $"/{userId}", token);
    }

    public HttpStatusCode CreateMultipleUser(AuthToken token, List<UserModel> users)
    {
        string userJson = JsonConvert.SerializeObject(users);
        return SendDataByUrl(HttpMethod.Post, _userController + "/all", token, userJson);
    }

    public HttpStatusCode UpdateUser(AuthToken token, UserModel user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        return SendDataByUrl(HttpMethod.Patch, _userController + $"/{user.Id}", token, userJson);
    }
}