using TaskManager.Common.Models;
using TaskManagerApi.Models.Abstractions;
using TaskManagerApi.Models.Data;

namespace TaskManagerApi.Models.Services;

public class TasksService : AbstractionService, ICommonService<TaskModel>
{
    private readonly ApplicationContext _db;

    public TasksService(ApplicationContext db)
    {
        _db = db;
    }


    public bool Create(TaskModel model)
    {
        return DoAction(delegate ()
        {
            Task newTask = new Task(model);
            _db.Tasks.Add(newTask);
            _db.SaveChanges();
        });
    }

    public bool Update(int id, TaskModel model)
    {
        return DoAction(delegate ()
        {
            Task task = _db.Tasks.FirstOrDefault(t => t.Id == id);

            task.Name = model.Name;
            task.Description = model.Description;
            task.Photo = model.Photo;
            task.StartDate = model.StartDate;
            task.EndDate = model.EndDate;
            task.DeskId = model.DeskId;
            task.Column = model.Column;
            task.CreatorId = model.CreatorId;
            task.ExecutorId = model.ExecutorId;
            task.File = model.File;
           
            _db.Tasks.Update(task);
            _db.SaveChanges();
        });
    }

    public bool Delete(int id)
    {
        return DoAction(delegate ()
        {
            Task task = _db.Tasks.FirstOrDefault(t => t.Id == id);
            _db.Tasks.Remove(task);
            _db.SaveChanges();
        });
    }

    public TaskModel Get(int id)
    {
        Task task = _db.Tasks.FirstOrDefault(t => t.Id == id);
        return task?.ToDto();
    }

    public IQueryable<CommonModel> GetAll(int deskId)
    {
        return _db.Tasks.Where(t => t.DeskId == deskId).Select(t => t.ToDto() as CommonModel);
    }

    public IQueryable<CommonModel> GetTasksForUser(int userId)
    { 
        return _db.Tasks.Where(t => t.CreatorId == userId || t.ExecutorId == userId).Select(t => t.ToDto() as CommonModel);
    }
}