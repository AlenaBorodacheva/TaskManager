using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests;

[TestClass()]
public class TasksRequestServiceTests
{
    private AuthToken _token;
    private TasksRequestService _service;

    public TasksRequestServiceTests()
    {
        _token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        _service = new TasksRequestService();
    }

    [TestMethod()]
    public void GetAllTasksTest()
    {
        var tasks = _service.GetAllTasks(_token);
        Console.WriteLine(tasks.Count);

        Assert.AreNotEqual(0, tasks.Count);
    }

    [TestMethod()]
    public void GetTaskByIdTest()
    {
        var task = _service.GetTaskById(_token, 7);
        Assert.AreNotEqual(null, task);
    }

    [TestMethod()]
    public void GetTasksByDeskTest()
    {
        var task = _service.GetTasksByDesk(_token, 3);
        Assert.AreNotEqual(0, task.Count);
    }

    [TestMethod()]
    public void CreateTaskTest()
    {
        var task = new TaskModel("Задача 1", "Задача из тестов", DateTime.Now, DateTime.Now.AddDays(7), "New");
        task.DeskId = 3;
        task.ExecutorId = 1;
        var result = _service.CreateTask(_token, task);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void UpdateTaskTest()
    {
        var task = new TaskModel("Задача 1.1", "Задача из тестов обновленная", DateTime.Now, DateTime.Now.AddDays(4), "In Progress");
        task.Id = 8;
        task.ExecutorId = 2;
        var result = _service.UpdateTask(_token, task);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void DeleteTaskByIdTest()
    {
        var result = _service.DeleteTask(_token, 7);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}