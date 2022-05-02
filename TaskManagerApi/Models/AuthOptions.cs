using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskManager.Api.Models;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    private const string KEY = "mysupersecret_secretkey!123";
    public const int LIFETIME = 1; // в минутах 

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}