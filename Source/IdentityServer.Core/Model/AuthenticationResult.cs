using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Model
{
    /// <summary>
    /// Result of the authentication function of <see cref="Providers.IAuthenticationProvider"/> 
    /// </summary>
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public bool IsServerError { get; set; }
        public string ErrorMessage { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
