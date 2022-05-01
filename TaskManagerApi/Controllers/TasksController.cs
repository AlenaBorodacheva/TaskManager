using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Common.Models;
using TaskManagerApi.Models.Data;
using TaskManagerApi.Models.Services;

namespace TaskManagerApi.Controllers;

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
    public async Task<IEnumerable<CommonModel>> GetTasksByDesk(int deskId)
    {
        return await _tasksService.GetAll(deskId).ToListAsync();
    }
    
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var desk = _tasksService.Get(id);
        return desk == null ? NotFound() : Ok(desk);
    }

    [HttpGet("user")]
    public async Task<IEnumerable<CommonModel>> GetTasksForCurrentUser()
    {
        var user = _usersService.GetUser(HttpContext.User.Identity.Name);
        if (user != null)
        {
            return await _tasksService.GetTasksForUser(user.Id).ToListAsync();
        }

        return Array.Empty<CommonModel>();
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