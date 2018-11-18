using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Core.Model;
using IdentityServer.Core.Providers;
using Microsoft.Extensions.Logging;
using IdentityServer.API;
using System.Security.Claims;
using System.Security.Principal;
using IdentityServer.API.JWT;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Concurrent;
using IdentityServer.Core;
using IdentityServer.API.Response;
using IdentityServer.API.Request;
using Microsoft.AspNetCore.Cors;

namespace AuthenticaitonAPI.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {

        private readonly IAuthenticationProvider _authenticationProvider = null;
        private readonly ILogger _logger = null;

        public AuthenticationController(IAuthenticationProvider authenticationProvider, ILoggerFactory loggerFactory)
        {
            _authenticationProvider = authenticationProvider;
            _logger = loggerFactory.CreateLogger<AuthenticationController>();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<LoginResponse> Login([FromBody]LoginRequest loginRequest)
        {
            var response = new LoginResponse();

            try
            {
                if (loginRequest == null || (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password)))
                    throw new APIException("Credentials cannot be null");

                string error = string.Empty;
                ClaimsIdentity identity = await GetClaimsIdentity(loginRequest, out error);
                var jwtSecurityToken = await JwtTokenManager.GetJwtTokenForIdentity(loginRequest, identity);
                response.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                response.Expires = jwtSecurityToken.ValidTo;
                SaveResponseOfClient(response);
                response.IsSuccess = true;
                response.ErrorMessage = error;
                response.Name = identity.FindFirst("Name").Value;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Login failed.";
                LogException(IdentityServer.API.EventIds.Login, ex);
            }

            return response;
            //--validation         

        }

        [Route("[action]")]
        [HttpPost]
        public VerifyResponse Verify([FromBody]VerifyRequest verifyRequest)
        {
            var response = new VerifyResponse();
            try
            {
                if (verifyRequest == null || string.IsNullOrWhiteSpace(verifyRequest.Token))
                    throw new APIException("Token cannot be null");

                LoginResponse loginResponse = null;
                loginResponse = Storage.IssuedTokens.SingleOrDefault(t => t.Token == verifyRequest.Token);
                if (loginResponse != null)
                {
                    if (JwtTokenManager.ValidateToken(loginResponse.Token))
                    {
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Invalid token.";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = "User not authenticated.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Verification failed.";
                LogException(IdentityServer.API.EventIds.Verify, ex);

            }
            return response;
        }


        [Route("[action]")]
        [HttpPost]
        public LogoutResponse Logout([FromBody]LogoutRequest logoutRequest)
        {
            var response = new LogoutResponse();
            try
            {
                if (logoutRequest == null || string.IsNullOrWhiteSpace(logoutRequest.Token))
                    throw new APIException("Token cannot be null");

                LoginResponse loginResponse = null;
                loginResponse = Storage.IssuedTokens.SingleOrDefault(t => t.Token == logoutRequest.Token);
                if (loginResponse != null)
                    Storage.IssuedTokens.TryTake(out loginResponse);

                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Logout failed.";
                LogException(IdentityServer.API.EventIds.Logout, ex);
            }
            return response;
        }

        private void LogException(EventId eventId, Exception ex)
        {
            if (!(ex is APIException))
                _logger.LogCritical(eventId, ex, string.Empty);

        }

        private Task<ClaimsIdentity> GetClaimsIdentity(Credentials credentials, out string error)
        {
            AuthenticationResult result = _authenticationProvider.AuthenticateUser(credentials);
            error = result.ErrorMessage;

            return (result.IsSuccess) ? Task.FromResult(new ClaimsIdentity(
                  new GenericIdentity(credentials.Username, "Token"),
                  new[]
                  {
                            new Claim(nameof(result.UserProfile.Name), result.UserProfile.Name),

                  })) : Task.FromResult<ClaimsIdentity>(null);
        }

        private void SaveResponseOfClient(LoginResponse response)
        {
            Storage.IssuedTokens.Add(response);
        }

    }

}
