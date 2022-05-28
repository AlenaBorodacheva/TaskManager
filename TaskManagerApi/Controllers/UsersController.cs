using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models.Data;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]  // [AllowAnonymous] - для исключения
public class UsersController : ControllerBase
{
    private readonly ApplicationContext _db;
    private readonly UsersService _usersService;

    public UsersController(ApplicationContext db)
    {
        _db = db;
        _usersService = new UsersService(db);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserModel userModel)
    {
        if (userModel != null)
        {
            bool result = _usersService.Create(userModel);
            return result ? Ok() : NotFound();
        }

        return BadRequest();
    }
    
    [HttpPatch("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
    {
        if (userModel != null)
        {
            bool result = _usersService.Update(id, userModel);
            return result ? Ok() : NotFound();
        }

        return BadRequest();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        bool result = _usersService.Delete(id);
        return result ? Ok() : NotFound();
    }

    [AllowAnonymous]
    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<UserModel> GetUserById(int id)
    {
        var result = _usersService.Get(id);
        return User == null? NotFound() : Ok(result);
    }

    [AllowAnonymous]
    [Authorize]
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
            bool result = _usersService.CreateMultipleUsers(userModels);
            return result ? Ok() : NotFound();
        }

        return BadRequest();
    }

    [AllowAnonymous]
    [Authorize]
    [HttpGet("{id}/admin")]
    public ActionResult<int> GetProjectAdminId(int id)
    {
        var admin = _usersService.GetProjectAdmin(id);
        return admin == null ? NotFound(null) : Ok(admin.Id);
    }
}