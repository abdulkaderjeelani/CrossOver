using IdentityServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Providers
{
    /// <summary>
    /// Provider for authentication mechanism, Implement this interface to provide your custom authentication
    /// </summary>
    public interface IAuthenticationProvider
    {
        AuthenticationResult AuthenticateUser(Credentials credentials);
    }
}
