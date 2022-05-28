using System.IO;
using System.Net;
using System.Windows;
using Microsoft.Win32;
using Prism.Mvvm;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class CommonViewService
{
    private string _imageDialogFilterPattern = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
    public Window CurrentOpenWindow { get; set; }

    public CommonViewService()
    {

    }

    public void ShowMessage(string message)
    {
        MessageBox.Show(message);
    }

    public void ShowActionResult(HttpStatusCode statusCode, string message)
    {
        if (statusCode == HttpStatusCode.OK)
        {
           ShowMessage(statusCode.ToString() + $"\n{message}");
        }
        else
        {
            ShowMessage(statusCode.ToString() + "Error");
        }
    }

    public void OpenWindow(Window wnd, BindableBase viewModel)
    {
        CurrentOpenWindow = wnd;
        wnd.DataContext = viewModel;
        wnd.ShowDialog();
    }

    public string GetFileFromDialog(string filter)
    {
        string filePath = "";
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = filter;
        bool? result = dlg.ShowDialog();

        if (result == true)
        {
            filePath = dlg.FileName;
        }

        return filePath;
    }

    public CommonModel SetPhotoForObject(CommonModel model)
    {
        string photoPath = GetFileFromDialog(_imageDialogFilterPattern);
        if (string.IsNullOrEmpty(photoPath) == false)
        {
            var photoBytes = File.ReadAllBytes(photoPath);
            model.Photo = photoBytes;
        }
        return model;
    }
}