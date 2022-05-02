using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models.Data;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ApplicationContext _db;
    private readonly UsersService _usersService;
    private readonly TasksService _tasksService;

    public TasksController(ApplicationContext db)
    {
        _db = db;
        _usersService = new UsersService(db);
        _tasksService = new TasksService(db);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommonModel>>> GetTasksByDesk(int deskId)
    {
        var result = await _tasksService.GetAll(deskId).ToListAsync();
        return result == null ? NoContent() : Ok(result);
    }
    
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var desk = _tasksService.Get(id);
        return desk == null ? NotFound() : Ok(desk);
    }

    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<CommonModel>>> GetTasksForCurrentUser()
    {
        var user = _usersService.GetUser(HttpContext.User.Identity.Name);
        if (user != null)
        {
            var result = await _tasksService.GetTasksForUser(user.Id).ToListAsync();
            return result == null ? NoContent() : Ok(result);
        }

        return Unauthorized(Array.Empty<CommonModel>());
    }

    [HttpPost]
    public IActionResult Create([FromBody] TaskModel model)
    {
        var user = _usersService.GetUser(HttpContext.User.Identity.Name);
        if (user != null)
        {
            if (model != null)
            {
                model.CreatorId = user.Id;
                bool result = _tasksService.Create(model);
                return result ? Ok() : NotFound();
            }

            return BadRequest();
        }

        return Unauthorized();
    }
    
    [HttpPatch("{id}")]
    public IActionResult Put(int id, [FromBody] TaskModel model)
    {
        var user = _usersService.GetUser(HttpContext.User.Identity.Name);
        if (user != null)
        {
            if (model != null)
            {
                bool result = _tasksService.Update(id, model);
                return result ? Ok() : NotFound();
            }

            return BadRequest();
        }

        return Unauthorized();
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        bool result = _tasksService.Delete(id);
        return result ? Ok() : NotFound();
    }
}