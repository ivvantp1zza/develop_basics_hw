using HttpServer.Attributes;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace HttpServer.ORM;


public class MyORM
{
    private readonly string _connectionString;

    public MyORM(string dbName)
    {
        _connectionString =
            @$"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True";
    }

    public List<T> Select<T>()
    {
        var res = new List<T>();

        string sqlExpression = $"SELECT * FROM {typeof(T).Name}s";

        using SqlConnection connection = new SqlConnection(_connectionString);

        connection.Open();
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        using SqlDataReader reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            res = GetInstances<T>(reader).ToList();
            return res;
        }
        return res;
    }

    public T Select<T>(int id)
    {
        string sqlExpression = $"select * from {typeof(T).Name}s where Id = {id}";

        using SqlConnection connection = new SqlConnection(_connectionString);

        connection.Open();
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        using SqlDataReader reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            return GetInstances<T>(reader).ToList().FirstOrDefault();
        }

        return default;
    }

    public T Select<T>(string columnName, object value)
    {
        string sqlExpression = $"select * from {typeof(T).Name}s where {columnName} = {value}";

        using SqlConnection connection = new SqlConnection(_connectionString);

        connection.Open();
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        using SqlDataReader reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            return GetInstances<T>(reader).ToList().FirstOrDefault();
        }

        return default;
    }

    public void Insert<T>(T value)
    {
        var model = typeof(T);
        var props = model.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.IsDefined(typeof(NotIdentityValue), true))
            .ToDictionary(p => p.GetCustomAttribute<NotIdentityValue>()!.PropName, pr => pr.GetValue(value));
        string sqlExpression = $"insert into {model.Name}s " +
                               $"({string.Join(", ", props.Keys)}) " +
                               $"values ('{string.Join("', '", props.Values)}')";

        using SqlConnection connection = new SqlConnection(_connectionString);

        connection.Open();
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        command.ExecuteNonQuery();
    }

    public void Delete<T>()
    {
        var model = typeof(T);

        string sqlExpression = $"delete from {model.Name}s";

        using SqlConnection connection = new SqlConnection(_connectionString);

        connection.Open();
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        command.ExecuteNonQuery();
    }

    public void Delete<T>(int id)
    {
        var model = typeof(T);

        string sqlExpression = $"delete from {model.Name}s where Id = {id}";

        using SqlConnection connection = new SqlConnection(_connectionString);

        connection.Open();
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        command.ExecuteNonQuery();
    }

    public void Update<T>(int id, string columnName, object value)
    {
        var model = typeof(T);

        string sqlExpression = $"update {model.Name}s set {columnName} = '{value}' where Id = {id}";

        using SqlConnection connection = new SqlConnection(_connectionString);

        connection.Open();
        SqlCommand command = new SqlCommand(sqlExpression, connection);
        command.ExecuteNonQuery();
    }

    private static IEnumerable<T> GetInstances<T>(IDataReader reader)
    {
        var ctor = typeof(T).GetConstructors()
            .FirstOrDefault(ctor => Attribute.IsDefined(ctor, typeof(DbCtor)));
        if (ctor is null)
            throw new InvalidDataException();
        while (reader.Read())
            yield return (T)ctor.Invoke(GetValuesFromReader(reader).ToArray());
        reader.Close();
    }

    private static IEnumerable<object> GetValuesFromReader(IDataRecord reader)
    {
        return Enumerable
            .Range(0, reader.FieldCount)
            .Select(i => reader.GetValue(i));
    }
}
