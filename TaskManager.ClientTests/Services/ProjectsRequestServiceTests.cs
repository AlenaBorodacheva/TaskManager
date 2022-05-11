using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests;

[TestClass()]
public class ProjectsRequestServiceTests
{
    private AuthToken _token;
    private ProjectsRequestService _service;

    public ProjectsRequestServiceTests()
    {
        _token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        _service = new ProjectsRequestService();
    }

    [TestMethod()]
    public void GetAllProjectsTest()
    {
        var projects = _service.GetAllProjects(_token);
        Console.WriteLine(projects.Count);

        Assert.AreNotEqual(Array.Empty<ProjectModel>(), projects);
    }

    [TestMethod()]
    public void GetProjectByIdTest()
    {
        var project = _service.GetProjectById(_token, 1);

        Assert.AreNotEqual(null, project);
    }

    [TestMethod()]
    public void CreateProjectTest()
    {
        ProjectModel project = new ProjectModel("Тестовый проект", "Описание нового тестового проекта", ProjectStatus.InProgress);
        project.AdminId = 1;
        var result = _service.CreateProject(_token, project);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void UpdateProjectTest()
    {
        ProjectModel project = new ProjectModel("Тестовый проект обновленный", "Описание нового тестового проекта обновленного", ProjectStatus.Suspended);
        project.Id = 1;
        var result = _service.UpdateProject(_token, project);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void DeleteProjectTest()
    {
        var result = _service.DeleteProject(_token, 2);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void AddUsersToProjectTest()
    {
        var result = _service.AddUsersToProject(_token, 1, new List<int>(){5, 6, 7});

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void RemoveUsersFromProjectTest()
    {
        var result = _service.RemoveUsersFromProject(_token, 1, new List<int>(){5, 6});

        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}