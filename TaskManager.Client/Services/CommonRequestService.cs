using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services;

public abstract class CommonRequestService
{
    public const string HOST = "http://localhost:52012/api/";

    public string GetDataByUrl(HttpMethod method, string url, AuthToken token, string userName = null, string password = null)
    {
        string result = string.Empty;
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        request.Method = method.Method;
        if (userName != null && password != null)
        {
            string encoded =
                Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
        }
        else if (token != null)
        {
            request.Headers.Add("Authorization", "Bearer " + token.access_token);
        }

        HttpWebResponse responce = (HttpWebResponse)request.GetResponse();
        using (StreamReader reader = new StreamReader(responce.GetResponseStream(), Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }

        return result;
    }

    public HttpStatusCode SendDataByUrl(HttpMethod method, string url, AuthToken token, string data = null)
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

    public HttpStatusCode DeleteDataByUrl(string url, AuthToken token)
    {
        HttpResponseMessage result = new HttpResponseMessage();
        HttpClient client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
        result = client.DeleteAsync(url).Result;

        return result.StatusCode;
    }
}