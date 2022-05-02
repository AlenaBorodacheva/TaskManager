using TaskManager.Common.Models;

namespace TaskManager.Api.Models;

public class User
{
    public int Id { get; set; }    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Phone { get; set; }
    public DateTime RegistraionDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public byte[]? Photo { get; set; }
    public List<Project> Projects { get; set; } = new List<Project>();
    public List<Desk> Desks { get; set; } = new List<Desk>();
    public List<Task> Tasks { get; set; } = new List<Task>();
    public UserStatus Status { get; set; }

    public User()
    {
        
    }

    public User(UserModel model)
    {
        FirstName = model.FirstName;
        LastName = model.LastName;
        Email = model.Email;
        Password = model.Password;
        Status = model.Status;
        Phone = model.Phone;
        Photo = model.Photo;
    }

    public User(string fname, string lname, string email, string password, 
        UserStatus status = UserStatus.User, string phone= null, byte[] photo = null)
    {
        FirstName = fname;
        LastName = lname;
        Email = email;
        Password = password;
        Status = status;
        Phone = phone;
        Photo = photo;
        RegistraionDate = DateTime.Now;
    }

    public UserModel ToDto()
    {
        return new UserModel()
        {
            Id = this.Id,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            Password = this.Password,
            Status = this.Status,
            Phone = this.Phone,
            Photo = this.Photo
        };
    }
}