using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Model
{
    /// <summary>
    /// User Crendials to authenticate
    /// </summary>
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }       
    }
}
