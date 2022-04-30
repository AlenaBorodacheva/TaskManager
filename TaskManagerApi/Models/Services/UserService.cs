using System.Security.Claims;
using System.Text;
using TaskManagerApi.Models.Data;

namespace TaskManagerApi.Models.Services;

public class UserService
{
    private readonly ApplicationContext _db;

    public UserService(ApplicationContext db)
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
}