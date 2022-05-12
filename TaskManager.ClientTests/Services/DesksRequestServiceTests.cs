using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests;

[TestClass()]
public class DesksRequestServiceTests
{
    private AuthToken _token;
    private DesksRequestService _service;

    public DesksRequestServiceTests()
    {
        _token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        _service = new DesksRequestService();
    }

    [TestMethod()]
    public void GetAllDesksTest()
    {
        var desks = _service.GetAllDesks(_token);
        Console.WriteLine(desks.Count);

        Assert.AreNotEqual(Array.Empty<DeskModel>(), desks);
    }

    [TestMethod()]
    public void GetDeskByIdTest()
    {
        var desk = _service.GetDeskById(_token, 2);
        Assert.AreNotEqual(null, desk);
    }

    [TestMethod()]
    public void GetDesksByProjectTest()
    {
        var desks = _service.GetDesksByProject(_token, 1);
        Assert.AreNotEqual(0, desks.Count);
    }

    [TestMethod()]
    public void CreateDeskTest()
    {
        var desk = new DeskModel("Доска из тестов", "Доска для тестирования", true, new string[] { "Новые", "Готовые" });
        desk.ProjectId = 1;
        desk.AdminId = 1;
        var result = _service.CreateDesk(_token, desk);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void UpdateDeskTest()
    {
        var desk = new DeskModel("Доска из тестов", "Доска для тестирования обновленная", true, new string[] { "Новые", "На проверке", "Готовые" });
        desk.ProjectId = 1;
        desk.AdminId = 1;
        desk.Id = 3;
        var result = _service.UpdateDesk(_token, desk);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void DeleteDeskTest()
    {
        var result = _service.DeleteDesk(_token, 2);
        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}