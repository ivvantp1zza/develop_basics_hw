using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using HttpServer.Sessions;
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

    [HttpGET("profile")]
    public Account GetAccountInfo(int id)
    {
        var dao = new DAO("AppDB");
        return dao.SelectById<Account>(id);
    }
    
    [HttpPOST]
    public Guid Login(string login, string password)
    {
        
        var dao = new DAO("AppDB");
        var accounts = dao.Select<Account>();
        var acc = accounts.FirstOrDefault(x => x.Login == login && x.Password == password);
        var sessionId = acc is not null ? SessionManager.CreateSession(acc.Id, login, DateTime.Now) : Guid.Empty;
        return sessionId;
    }
}