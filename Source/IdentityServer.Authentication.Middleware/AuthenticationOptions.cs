using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Authentication.Middleware
{
    public class AuthenticationOptions
    {
        public string LoginRedirectUrl { get; set; }
        public string LogoutPath { get; set; }
        public string LaunchPath { get; set; }
    }
}
