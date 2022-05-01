using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Common.Models;
using TaskManagerApi.Models.Data;
using TaskManagerApi.Models.Services;

namespace TaskManagerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]  // [AllowAnonymous] - для исключения
public class UsersController : ControllerBase
{
    private readonly ApplicationContext _db;
    private readonly UserService _userService;

    public UsersController(ApplicationContext db)
    {
        _db = db;
        _userService = new UserService(db);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserModel userModel)
    {
        if (userModel != null)
        {
            bool result = _userService.Create(userModel);
            return result ? Ok() : NotFound();
        }

        return BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
    {
        if (userModel != null)
        {
            bool result = _userService.Update(id, userModel);
            return result ? Ok() : NotFound();
        }

        return BadRequest();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        bool result = _userService.Delete(id);
        return result ? Ok() : NotFound();
    }

    [HttpGet]
    public async Task<IEnumerable<UserModel>> GetUsers()
    {
        return await _db.Users.Select(u => u.ToDto()).ToListAsync();
    }

    [HttpPost("all")]
    public IActionResult CreateMultipleUsers([FromBody] List<UserModel> userModels)
    {
        if (userModels != null && userModels.Count > 0)
        {
            bool result = _userService.CreateMultipleUsers(userModels);
            return result ? Ok() : NotFound();
        }

        return BadRequest();
    }
}