﻿using TaskManager.Common.Models;
using TaskManagerApi.Models.Abstractions;
using TaskManagerApi.Models.Data;

namespace TaskManagerApi.Models.Services;

public class ProjectsService : AbstractionService, ICommonService<ProjectModel>
{
    private readonly ApplicationContext _db;

    public ProjectsService(ApplicationContext db)
    {
        _db = db;
    }

    public bool Create(ProjectModel model)
    {
        return DoAction(delegate()
        {
            Project newProject = new Project(model);
            _db.Projects.Add(newProject);
            _db.SaveChanges();
        });
    }

    public bool Update(int id, ProjectModel model)
    {
        return DoAction(delegate()
        {
            Project newProject = _db.Projects.FirstOrDefault(p => p.Id == id);
            newProject.Name = model.Name;
            newProject.Description = model.Description;
            newProject.Photo = model.Photo;
            newProject.Status = model.Status;
            newProject.AdminId = model.AdminId;
            _db.Projects.Update(newProject);
            _db.SaveChanges();
        });
    }

    public bool Delete(int id)
    {
        return DoAction(delegate ()
        {
            Project newProject = _db.Projects.FirstOrDefault(p => p.Id == id);
            if (newProject != null)
            {
                _db.Projects.Remove(newProject);
                _db.SaveChanges();
            }
        });
    }
}