using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using IdentityServer.Interceptor.Core;

namespace IdentityServer.Authentication.Middleware
{
    /// <summary>
    /// Middleware to enable single signon / signoff to your application,
    /// Signon Uri http://localhost:5001/login 
    /// The client application must have a Launch Url (mandatory). 
    /// This Url will be the entry point of application.
    /// Needed as we maintain access_token in browser storage !!!
    /// </summary>
    public class AuthenticationMiddleware
    {

        private static readonly EventId FailedEventId = new EventId(1000, nameof(AuthenticationMiddleware));


        private readonly RequestDelegate _next;
        private readonly AuthenticationOptions _options;
        private readonly ILogger _logger;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, AuthenticationOptions options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<AuthenticationMiddleware>();
            _options = options;
            if (_authenticationService == null)
                _authenticationService = new AuthenticationService();
        }

#if DEBUG

        /// <summary>
        /// This constructor should be available on production, !!! IMPORTANT !!!
        /// Only for unit tests
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        /// <param name="authenticationService"></param>
        public AuthenticationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, AuthenticationOptions options, IAuthenticationService authenticationService)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<AuthenticationMiddleware>();
            _options = options;
            _authenticationService = authenticationService;
        }
#endif
        public async Task Invoke(HttpContext context)
        {
            try
            {
                //No verifications for the launch page
                if (context.Request.Path.Value.Trim().ToUpperInvariant() == _options.LaunchPath.ToUpperInvariant())
                {
                    await _next(context);
                }
                else if(context.Request.Path.Value.Trim().ToUpperInvariant() == _options.LogoutPath.ToUpperInvariant())
                {
                    //retrieve the access token from the request
                    string accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrWhiteSpace(accessToken))
                        await Logout(accessToken);

                    RedirectToLogin(context);
                }
                else
                {
                    bool isAuthenticated = false;

                    //retrieve the access token from the request
                    string accessToken = context.Request.Query["access_token"];                    
                    
                    //verify the token
                    if (!string.IsNullOrEmpty(accessToken))
                        isAuthenticated = await VerifyToken(accessToken);

                    //redirect if not authenticated
                    if (!isAuthenticated)
                        RedirectToLogin(context);
                    else
                        await _next(context);

                }
            }
            catch (Exception ex)
            {
                //Make sure even if there is any error in logger, the request is propagated to next middleware in pipeline
                try { _logger.LogCritical(FailedEventId, ex, "Unexpected exception"); } catch { }
                await _next(context);
            }
        }


        private void RedirectToLogin(HttpContext context)
        {
            _authenticationService.RedirectToLogin(_options.LoginRedirectUrl,
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
                _logger.LogCritical(FailedEventId, ex, "Failed to verify token");
                return false;
            }
        }

        /// <summary>
        /// Logs out of the system, calling authentication api
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private async Task Logout(string accessToken)
        {
            try
            {
                await _authenticationService.Logout(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(FailedEventId, ex, "Failed to logout");               
            }
        }
    }
}
