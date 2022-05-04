using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests;

[TestClass()]
public class UsersRequestServiceTests
{
    [TestMethod()]
    public void GetTokenTest()
    {
        var token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        Console.WriteLine(token.access_token);

        Assert.IsNotNull(token.access_token);
    }

    [TestMethod()]
    public void CreateUserTest()
    {
        var service = new UsersRequestService();
        var token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        UserModel userTest = new UserModel("Vasya", "Pupkin", "vasya@mail.ru", "qwerty", UserStatus.User, "1234");
        var result = service.CreateUser(token, userTest);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void GetAllUsersTest()
    {
        var service = new UsersRequestService();
        var token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        var result = service.GetAllUsers(token);

        Assert.AreNotEqual(Array.Empty<UserModel>(), result.ToArray());
    }

    [TestMethod()]
    public void DeleteUserTest()
    {
        var service = new UsersRequestService();
        var token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        var result = service.DeleteUser(token, 3);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void CreateMultipleUsersTest()
    {
        var service = new UsersRequestService();
        var token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");

        UserModel userTest1 = new UserModel("Vasya", "Pupkin", "Pupkin@mail.ru", "12345", UserStatus.User, "8915");
        UserModel userTest2 = new UserModel("Alex", "Muller", "Muller@mail.ru", "qwerty", UserStatus.Editor, "8910");
        UserModel userTest3 = new UserModel("Petya", "Ivanov", "Ivanov@mail.ru", "zxcvb", UserStatus.User, "8920");
        List<UserModel> users = new List<UserModel>() {userTest1, userTest2, userTest3};

        var result = service.CreateMultipleUsers(token, users);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }

    [TestMethod()]
    public void UpdateUserTest()
    {
        var service = new UsersRequestService();
        var token = new UsersRequestService().GetToken("alien_borodach@mail.ru", "123");
        UserModel user = new UserModel("Petya", "Ivanov", "Ivanov@mail.ru", "zxcvb", UserStatus.User, "88005553535");
        user.Id = 9;
        var result = service.UpdateUser(token, user);

        Assert.AreEqual(HttpStatusCode.OK, result);
    }
}