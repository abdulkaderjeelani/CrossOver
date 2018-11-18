using System;
using System.Threading.Tasks;

namespace IdentityServer.Interceptor.Core
{
    public interface IAuthenticationService
    {
        Task<bool> Logout(string accessToken);
        Task<bool> VerifyToken(string accessToken);
        void RedirectToLogin(string returnUrl, Action<string> redirector);
    }
}