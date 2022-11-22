using HttpServer.Attributes;

namespace HttpServer.Models;

public class Account
{
    public int Id { get; set; }
    [NotIdentityValue("Login")]
    public string Login { get; set; }
    [NotIdentityValue("Password")]
    public string Password { get; set; }

    [DbCtor]
    public Account(int id, string login, string password)
    {
        Id = id;
        Login = login;
        Password = password;
    }

    public Account(string login, string password)
    {
        Login = login;
        Password = password;
    }
}