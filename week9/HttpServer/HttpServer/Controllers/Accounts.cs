using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using System.Data.SqlClient;
using System.Net;

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
    public int Login(string login, string password)
    {
        var dao = new DAO("AppDB");
        var accounts = dao.Select<Account>();
        var acc = accounts.FirstOrDefault(x => x.Login == login && x.Password == password);
        return acc is null ? -1 : acc.Id;
    }
}