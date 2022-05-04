using System.Security.Claims;
using System.Text;
using TaskManager.Api.Models.Abstractions;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services;

public class UsersService : AbstractionService, ICommonService<UserModel>
{
    private readonly ApplicationContext _db;

    public UsersService(ApplicationContext db)
    {
        _db = db;
    }

    public Tuple<string, string> GetUserLoginPassFromBasicAuth(HttpRequest request)
    {
        string userName = "";
        string userPass = "";
        string authHeader = request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic"))
        {
            string encodedUserNamePass = authHeader.Replace("Basic ", "");
            var encoding = Encoding.GetEncoding("iso-8859-1");
            string[] namePassArray = encoding.GetString(Convert.FromBase64String(encodedUserNamePass)).Split(":");
            userName = namePassArray[0];
            userPass = namePassArray[1];
        }

        return new Tuple<string, string>(userName, userPass);
    }

    public User GetUser(string login, string password)
    {
        return _db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
    }

    public User GetUser(string login)
    {
        return _db.Users.FirstOrDefault(u => u.Email == login);
    }

    public ClaimsIdentity GetIdentity(string userName, string password)
    {
        User currentUser = GetUser(userName, password);
        if (currentUser != null)
        {
            currentUser.LastLoginDate = DateTime.Now;
            _db.Users.Update(currentUser);
            _db.SaveChanges();

            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, currentUser.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, currentUser.Status.ToString())
            };

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        return null;
    }

    public bool Create(UserModel model)
    {
        return DoAction(delegate()
        {
            User newUser = new User(model.FirstName, model.LastName, model.Email,
                model.Password, model.Status, model.Phone, model.Photo);
            _db.Users.Add(newUser);
            _db.SaveChanges();
        });
    }

    public bool Update(int id, UserModel model)
    {
        return DoAction(delegate()
        {
            User userForUpdate = _db.Users.FirstOrDefault(u => u.Id == id);
            userForUpdate.FirstName = model.FirstName;
            userForUpdate.LastName = model.LastName;
            userForUpdate.Email = model.Email;
            userForUpdate.Password = model.Password;
            userForUpdate.Status = model.Status;
            userForUpdate.Phone = model.Phone;
            userForUpdate.Photo = model.Photo;
            _db.Users.Update(userForUpdate);
            _db.SaveChanges();
        });
    }

    public bool Delete(int id)
    {
        User userForDelete = _db.Users.FirstOrDefault(u => u.Id == id);
        if (userForDelete != null)
        {
            return DoAction(delegate ()
            {
                _db.Users.Remove(userForDelete);
                _db.SaveChanges();
            });
        }
        return false;
    }

    public bool CreateMultipleUsers(List<UserModel> userModels)
    {
        return DoAction(delegate()
        {
            var newUsers = userModels.Select(u => new User(u));
            _db.Users.AddRange(newUsers);
            _db.SaveChangesAsync();
        });
    }

    public UserModel Get(int id)
    {
        User user = _db.Users.FirstOrDefault(u => u.Id == id);
        return user?.ToDto();
    }

    public IEnumerable<UserModel> GetAllByIds(List<int> usersIds)
    {
        foreach (var id in usersIds)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id).ToDto();
            yield return user;
        }
    }
}