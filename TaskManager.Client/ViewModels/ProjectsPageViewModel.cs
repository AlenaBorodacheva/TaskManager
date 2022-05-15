using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Prism.Commands;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class ProjectsPageViewModel : BindableBase
{
    private AuthToken _token;
    private UsersRequestService _usersRequestService;
    private ProjectsRequestService _projectsRequestService;
    private CommonViewService _viewService;

    #region COMMANDS

    public DelegateCommand OpenNewProjectCommand { get; private set; }
    public DelegateCommand<object> OpenUpdateProjectCommand { get; private set; }
    public DelegateCommand<object> ShowProjectInfoCommand { get; private set; }
    public DelegateCommand CreateOrUpdateProjectCommand { get; private set; }
    public DelegateCommand DeleteProjectCommand { get; private set; }

    #endregion

    #region PROPERTIES

    private ClientAction _typeActionWithProject;

    public ClientAction TypeActionWithProject
    {
        get => _typeActionWithProject;
        set
        {
            _typeActionWithProject = value;
            RaisePropertyChanged(nameof(TypeActionWithProject));
        }
    }

    public List<ModelClient<ProjectModel>> UserProjects
    {
        get => _projectsRequestService.GetAllProjects(_token).Select(project => new ModelClient<ProjectModel>(project)).ToList();
    }

    private ModelClient<ProjectModel> _selectedProject;

    public ModelClient<ProjectModel> SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            RaisePropertyChanged(nameof(SelectedProject));

            if (SelectedProject.Model.AllUsersIds != null && SelectedProject.Model.AllUsersIds.Count > 0)
            {
                ProjectUsers = SelectedProject.Model.AllUsersIds?
                    .Select(userId => _usersRequestService.GetUserById(_token, userId)).ToList();
            }
            else
            {
                ProjectUsers = new List<UserModel>();
            }
        }
    }

    private List<UserModel> _projectUsers = new List<UserModel>();

    public List<UserModel> ProjectUsers
    {
        get => _projectUsers;
        set
        {
            _projectUsers = value;
            RaisePropertyChanged(nameof(ProjectUsers));
        }
    }

    #endregion

    public ProjectsPageViewModel(AuthToken token)
    {
        _viewService = new CommonViewService();
        _usersRequestService = new UsersRequestService();
        _projectsRequestService = new ProjectsRequestService();

        _token = token;
        OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
        OpenUpdateProjectCommand = new DelegateCommand<object>(OpenUpdateProject);
        ShowProjectInfoCommand = new DelegateCommand<object>(ShowProjectInfo);
        CreateOrUpdateProjectCommand = new DelegateCommand(CreateOrUpdateProject);
        DeleteProjectCommand = new DelegateCommand(DeleteProject);
    }

    #region METHODS

    private void OpenNewProject()
    {
        _typeActionWithProject = ClientAction.Create;
        var wnd = new CreateOrUpdateProjectWindow();
        wnd.DataContext = this;
        wnd.ShowDialog();
    }

    private void OpenUpdateProject(object projectId)
    {
        SelectedProject = GetProjectClientById(projectId);

        _typeActionWithProject = ClientAction.Update;
        var wnd = new CreateOrUpdateProjectWindow();
        wnd.DataContext = this;
        wnd.ShowDialog();
    }

    private void ShowProjectInfo(object projectId)
    {
        SelectedProject = GetProjectClientById(projectId);
    }

    private ModelClient<ProjectModel> GetProjectClientById(object projectId)
    {
        try
        {
            int id = (int)projectId;
            ProjectModel project = _projectsRequestService.GetProjectById(_token, id);
            return new ModelClient<ProjectModel>(project);
        }
        catch (FormatException)
        {
            return new ModelClient<ProjectModel>(null);
        }
    }

    public void CreateOrUpdateProject()
    {
        if (_typeActionWithProject == ClientAction.Create)
        {
            CreateProject();
        }

        if (_typeActionWithProject == ClientAction.Update)
        {
            UpdateProject();
        }
    }

    private void CreateProject()
    {
        var resultAction = _projectsRequestService.CreateProject(_token, SelectedProject.Model);

        if (resultAction == HttpStatusCode.OK)
        {
            _viewService.ShowMessage(resultAction.ToString() + "\nNew project is created");
        }
        else
        {
            _viewService.ShowMessage(resultAction.ToString() + "\nNew project is not created");
        }
    }

    private void UpdateProject()
    {
        var resultAction = _projectsRequestService.UpdateProject(_token, SelectedProject.Model);

        if (resultAction == HttpStatusCode.OK)
        {
            _viewService.ShowMessage(resultAction.ToString() + "\nProject is updated");
        }
        else
        {
            _viewService.ShowMessage(resultAction.ToString() + "\nProject is not updated");
        }
    }

    private void DeleteProject()
    {
        var resultAction = _projectsRequestService.DeleteProject(_token, SelectedProject.Model.Id);

        if (resultAction == HttpStatusCode.OK)
        {
            _viewService.ShowMessage(resultAction.ToString() + "\nProject is deleted");
        }
        else
        {
            _viewService.ShowMessage(resultAction.ToString() + "\nProject is not deleted");
        }
    }

    #endregion
}