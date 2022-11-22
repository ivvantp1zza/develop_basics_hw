using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class DAO
    {
        private MyORM _orm { get; }

        public DAO(string dbName)
        {
            _orm = new MyORM(dbName);
        }

        public List<T> Select<T>() => _orm.Select<T>();

        public T? SelectById<T>(int id) => _orm.Select<T>(id);

        public void Insert(Account account) => _orm.Insert(account);

        public void Delete() => _orm.Delete<Account>();

        public void DeleteById(int id) => _orm.Delete<Account>(id);

        public void Update(int id, string columnName, object value) => _orm.Update<Account>(id, columnName, value);
    }
}
