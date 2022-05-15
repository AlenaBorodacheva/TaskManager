using System.Windows;
using System.Windows.Controls;
using TaskManager.Client.ViewModels;
using TaskManager.Common.Models;

namespace TaskManager.Client.Views.AddWindows;

/// <summary>
/// Логика взаимодействия для AddUsersToProjectWindow.xaml
/// </summary>
public partial class AddUsersToProjectWindow : Window
{
    public AddUsersToProjectWindow()
    {
        InitializeComponent();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = (ProjectsPageViewModel)DataContext;
        foreach (UserModel user in e.RemovedItems)
        {
            viewModel.SelectedUsersForProject.Remove(user);
        }

        foreach (UserModel user in e.AddedItems)
        {
            viewModel.SelectedUsersForProject.Add(user);
        }
    }
}