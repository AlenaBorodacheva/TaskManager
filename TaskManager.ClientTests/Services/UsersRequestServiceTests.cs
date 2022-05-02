using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
}