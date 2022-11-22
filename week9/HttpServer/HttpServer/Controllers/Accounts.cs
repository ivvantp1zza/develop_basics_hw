using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using System.Data.SqlClient;

namespace HttpServer.Controllers;

[HttpController("accounts")]
public class Accounts
{
    [HttpGET]
    public List<Account> GetAccounts()
    {
        var dao = new DAO("AppDB");
        return dao.Select<Account>();
    }

    [HttpGET("getById")]
    public Account? GetAccountById(int id)
    {
        var dao = new DAO("AppDB");
        return dao.SelectById<Account>(id);
    }
    
    [HttpPOST]
    public string SaveAccount(string login, string password)
    {
        var dao = new DAO("AppDB");
        dao.Insert(new Account(login, password));
        return $"user {login} {password} successfully added";
    }
}