﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskManager.Common.Models;
using TaskManagerApi.Models.Abstractions;
using TaskManagerApi.Models.Data;

namespace TaskManagerApi.Models.Services;

public class DesksService : AbstractionService, ICommonService<DeskModel>
{
    private readonly ApplicationContext _db;

    public DesksService(ApplicationContext db)
    {
        _db = db;
    }

    public bool Create(DeskModel model)
    {
        return DoAction(delegate()
        {
            Desk newDesk = new Desk(model);
            _db.Desks.Add(newDesk);
            _db.SaveChanges();
        });
    }

    public bool Update(int id, DeskModel model)
    {
        return DoAction(delegate ()
        {
            Desk desk = _db.Desks.FirstOrDefault(d => d.Id == id);
            desk.Name = model.Name;
            desk.Description = model.Description;
            desk.Photo = model.Photo;
            desk.AdminId = model.AdminId;
            desk.IsPrivate = model.IsPrivate;
            desk.ProjectId = model.ProjectId;
            desk.Columns = JsonConvert.SerializeObject(model.Columns);
            _db.Desks.Update(desk);
            _db.SaveChanges();
        });
    }

    public bool Delete(int id)
    {
        return DoAction(delegate ()
        {
            Desk desk = _db.Desks.FirstOrDefault(d => d.Id == id);
            _db.Desks.Remove(desk);
            _db.SaveChanges();
        });
    }

    public DeskModel Get(int id)
    {
        Desk desk = _db.Desks.Include(d => d.Tasks).FirstOrDefault(x => x.Id == id);
        var deskModel = desk?.ToDto();
        if(deskModel != null)
        {
            deskModel.TasksIds = desk.Tasks.Select(t => t.Id).ToList();
        }

        return deskModel;
    }

    public IQueryable<CommonModel> GetAll(int userId)
    {
        return _db.Desks.Where(d => d.AdminId == userId).Select(d => d.ToDto() as CommonModel);
    }

    public IQueryable<CommonModel> GetProjectDesks(int projectId, int userId)
    {
        return _db.Desks.Where(d => d.ProjectId == projectId && 
                                     (d.AdminId == userId || d.IsPrivate == false)).Select(d => d.ToDto() as CommonModel);
    }
}