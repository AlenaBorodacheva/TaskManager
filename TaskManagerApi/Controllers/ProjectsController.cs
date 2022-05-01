﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Common.Models;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Data;
using TaskManagerApi.Models.Services;

namespace TaskManagerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationContext _db;
    private readonly ProjectsService _projectsService;
    private readonly UsersService _usersService;

    public ProjectsController(ApplicationContext db)
    {
        _db = db;
        _projectsService = new ProjectsService(db);
        _usersService = new UsersService(db);
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectModel>> Get()
    {
        return await _db.Projects.Select(p => p.ToDto()).ToListAsync();
    }

    [HttpPost]
    public IActionResult Create([FromBody] ProjectModel projectModel)
    {
        if (projectModel != null)
        {
            var user = _usersService.GetUser(HttpContext.User.Identity.Name);
            if (user != null)
            {
                var admin = _db.ProjectAdmins.FirstOrDefault(p => p.UserId == user.Id);
                if (admin == null)
                {
                    admin = new ProjectAdmin(user);
                    _db.ProjectAdmins.Add(admin);
                }
                projectModel.AdminId = admin.Id;
            }

            bool result = _projectsService.Create(projectModel);
            return result ? Ok() : NotFound();
        }
        return BadRequest();
    }

    [HttpPatch]
    public IActionResult Update(int id, [FromBody] ProjectModel projectModel)
    {
        if (projectModel != null)
        {
            bool result = _projectsService.Update(id, projectModel);
            return result ? Ok() : NotFound();
        }
        return BadRequest();
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        bool result = _projectsService.Delete(id);
        return result ? Ok() : NotFound();
    }
}