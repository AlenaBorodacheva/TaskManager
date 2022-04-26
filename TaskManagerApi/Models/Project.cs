using TaskManagerApi.Models.Abstractions;

namespace TaskManagerApi.Models;

public class Project : CommonObject
{
    public List<User> AllUsers { get; set; }
    public List<Desk> AllDesks { get; set; }
}