using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    internal class AccountRepository
    {
        private readonly Dictionary<int, Account> _repository;
        private readonly MyORM _orm;

        public AccountRepository(string dbName)
        {
            _orm = new MyORM(dbName);   
            _repository = _orm.Select<Account>().ToDictionary(key => key.Id, value => value);
        }

        public List<Account> Select() => _repository.Values.ToList();

        public Account Select(int id) => _repository[id];

        public void Insert(Account account)
        {
            _orm.Insert<Account>(account);
            _repository[account.Id] = account;
        }

        public void Delete()
        {
            _orm.Delete<Account>();
            _repository.Clear();
        }

        public void Delete(int id)
        {
            _orm.Delete<Account>(id);
            _repository.Remove(id);
        }

        public void Update(int id, string tableName, object newValue)
        {
            _orm.Update<Account>(id, tableName, newValue);
            var account = _repository[id];
            var property = account.GetType().GetProperty(tableName);
            property?.SetValue(account, Convert.ChangeType(newValue, property.PropertyType));
        }
    }
}
