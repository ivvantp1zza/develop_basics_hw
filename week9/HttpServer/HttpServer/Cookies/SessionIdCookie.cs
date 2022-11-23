using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Cookies
{
    internal class SessionIdCookie
    {
        public string IsAuthorize { get; set; }
        public int Id { get; set; }
        public bool _isAuth { get => IsAuthorize == "true"; }
        
    }
}
