using IdentityServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.API.Request
{
    public class LoginRequest : Credentials
    {
        public string ClientIp { get; set; }
    }
}
