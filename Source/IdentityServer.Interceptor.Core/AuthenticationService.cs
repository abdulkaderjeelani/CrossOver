using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Interceptor.Core
{
    public class AuthenticationService : IAuthenticationService
    {
        /*TODO: PLACE THESE CONSTANTS INFO IN A SECURE PLACE NOT IN CONF FILES*/
        private const string SingleSignOnUri = "http://localhost:5001/login";
        private const string LogoutUri = "http://localhost:5000/api/authentication/logout/";
        private const string TokenVerifyUri = "http://localhost:5000/api/authentication/verify/";
        private const string JsonMediaType = "application/json";

        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Verify the token, calling authentication api
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<bool> VerifyToken(string accessToken)
        {
            string verifyRequest = JsonConvert.SerializeObject(new { Token = accessToken });
            HttpResponseMessage verifyResponseMsg = await client.PostAsync(TokenVerifyUri, new StringContent(verifyRequest, Encoding.UTF8, JsonMediaType));
            verifyResponseMsg.EnsureSuccessStatusCode();
            string verifyResponseJson = await verifyResponseMsg.Content.ReadAsStringAsync();
            var verifyResponse = JsonConvert.DeserializeObject<IdentityServerResult>(verifyResponseJson);
            return verifyResponse.IsSuccess;

        }

        /// <summary>
        /// Logs out of the system, calling authentication api
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<bool> Logout(string accessToken)
        {

            string logoutRequest = JsonConvert.SerializeObject(new { Token = accessToken });
            HttpResponseMessage logoutResponseMsg = await client.PostAsync(LogoutUri, new StringContent(logoutRequest, Encoding.UTF8, JsonMediaType));
            logoutResponseMsg.EnsureSuccessStatusCode();
            string logoutResponseJson = await logoutResponseMsg.Content.ReadAsStringAsync();
            var logoutResponse = JsonConvert.DeserializeObject<IdentityServerResult>(logoutResponseJson);
            return logoutResponse.IsSuccess;

        }

        public void RedirectToLogin(string returnUrl, Action<string> redirector)
        {
            redirector($"{SingleSignOnUri}?returnUrl={returnUrl}");
        }
    }
}
