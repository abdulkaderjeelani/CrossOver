using IdentityServer.Interceptor.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
namespace IdentityServer.Authentication.HttpModule
{
    public class AuthenticationModule : IHttpModule
    {
        public static readonly string LoginRedirectUrl = WebConfigurationManager.AppSettings["LoginRedirectUrl"];
        public static readonly string LogoutPath = WebConfigurationManager.AppSettings["LogoutPath"];
        public static readonly string LaunchPath = WebConfigurationManager.AppSettings["LaunchPath"];
        public static readonly string LogDirectory = WebConfigurationManager.AppSettings["LogDirectory"];


        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += AuthenticateRequest;
        }

        IAuthenticationService _authenticationService = null;
        private async void AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                HttpApplication application = (HttpApplication)sender;
                HttpContext context = application.Context;
                string filePath = context.Request.FilePath;
                _authenticationService = new AuthenticationService();

                //No verifications for the launch page
                if (context.Request.Path.Trim().ToUpperInvariant().Contains(LaunchPath.ToUpperInvariant()))
                    return;

                //retrieve the access token from the request
                string accessToken = context.Request.QueryString["access_token"];

                if (context.Request.Path.Trim().ToUpperInvariant().Contains(LogoutPath.ToUpperInvariant()))
                {                    
                    if (!string.IsNullOrWhiteSpace(accessToken))
                        await Logout(accessToken);

                    RedirectToLogin(context);
                }
                else
                {
                    bool isAuthenticated = false;
                 
                    //verify the token
                    if (!string.IsNullOrEmpty(accessToken))
                        isAuthenticated = await VerifyToken(accessToken);

                    //redirect if not authenticated
                    if (!isAuthenticated)
                        RedirectToLogin(context);
                 
                }
                 
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void RedirectToLogin(HttpContext context)
        {
            _authenticationService.RedirectToLogin(LoginRedirectUrl,
               uri => context.Response.Redirect(uri));
        }


        /// <summary>
        /// Verify the token validity and also check whether the user has logged out of any other applicaton to 
        /// implement Single Sign Off
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<bool> VerifyToken(string accessToken)
        {
            try
            {
                return await _authenticationService.VerifyToken(accessToken);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        // <summary>
        /// Logs out of the system, calling authentication api
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task<bool> Logout(string accessToken)
        {
            try
            {
                return await _authenticationService.Logout(accessToken);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Writes errors to log path !May Use some logger
        /// </summary>
        /// <param name="ex"></param>
        private void LogException(Exception ex)
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            File.AppendAllText(Path.Combine(LogDirectory, $"{DateTime.Now.ToString("dd MMM yyyy")}.txt"), FormatException(ex));

        }

        public static string FormatException(Exception ex)
        {
            var exceptionBuilder = new StringBuilder();
            while (ex != null)
            {
                exceptionBuilder.AppendLine("       ");
                exceptionBuilder.AppendLine("Message: " + ex.Message);
                exceptionBuilder.AppendLine("Stack Trace: " + ex.StackTrace);
                ex = ex.InnerException;
            }
            exceptionBuilder.AppendLine("       ");
            exceptionBuilder.AppendLine(new string('-', 150));

            return exceptionBuilder.ToString(); ;
        }
    }
}
