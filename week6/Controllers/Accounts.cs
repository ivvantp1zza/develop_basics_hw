using HttpServer.Attributes;
using HttpServer.Models;
using System.Data.SqlClient;

namespace HttpServer.Controllers;

[HttpController("accounts")]
public class Accounts
{
    [HttpGET]
    public List<Account> GetAccounts()
    {
        List<Account> res = new List<Account>();

        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AppDB;Integrated Security=True;";

        string sqlExpression = "SELECT * FROM Accounts";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    res.Add(new Account()
                    {
                        Id = reader.GetInt32(0),
                        Login = reader.GetString(1),
                        Password = reader.GetString(2)
                    });
                }
            }
            reader.Close();
        }
        return res;
    }

    [HttpGET("getById")]
    public Account GetAccountById(int id)
    {
        Account res = null;
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AppDB;Integrated Security=True;";
        string sqlExpression = $"SELECT * FROM Accounts WHERE Id={id}";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    res = new Account()
                    {
                        Id = id,
                        Login = reader.GetString(1),
                        Password = reader.GetString(2)
                    };
                } 
            }
            reader.Close();
        }
        return res;
    }
    
    [HttpPOST]
    public string SaveAccount(string login, string password)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            return "login and password must be not empty!";
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AppDB;Integrated Security=True;";
        string sqlExpression = $"INSERT INTO Accounts (Login, Password) VALUES ('{login}', '{password}')";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            int changes = command.ExecuteNonQuery();
        }
        return login + "  " + password + " added succesfully";
    }
}