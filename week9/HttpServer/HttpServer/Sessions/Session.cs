using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Sessions
{
    public class Session
    {
        public readonly Guid Id;
        public readonly int AccountId;
        public readonly string Login;
        public readonly DateTime CreateDateTime;

        public Session(Guid id, int accountId, string login, DateTime created)
        {
            Id = id;
            AccountId = accountId;
            Login = login;
            CreateDateTime = created;
        }
    }
}
