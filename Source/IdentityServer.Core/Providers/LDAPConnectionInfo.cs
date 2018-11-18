using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Providers
{
    public class LDAPConnectionInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
