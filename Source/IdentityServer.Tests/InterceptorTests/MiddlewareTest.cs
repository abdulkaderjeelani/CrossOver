using IdentityServer.Authentication.Middleware;
using IdentityServer.Interceptor.Core;
using IdentityServer.Tests.APITests;
using IdentityServer.Tests.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.Tests.InterceptorTests
{
    public class MiddlewareTest
    {
        static readonly ILoggerFactory LoggerFactory = new LoggerFactoryTestDouble<AuthenticationMiddleware>();
        static readonly AuthenticationOptions AuthenticationOptions = new AuthenticationOptions()
        {
            LaunchPath = "/LAUNCH",
            LoginRedirectUrl = "LOGIN",
            LogoutPath = "/LOGOUT"

        };

        MockHttpContext _mockHttpContext = null;
        AuthenticationMiddleware _middleware = null;

        private string GetAccessToken()
        {
            return new AuthenticationTest().LoginUser().Result.Token;
        }

        string RedirectUrl = string.Empty;
        string LogoutToken = string.Empty;
        string VerifyToken = string.Empty;
        bool IsNextStepCalled = false;

        public void Initialize(string accessToken, Func<string> pathGetter, bool verifyResult = true, bool logoutResult = true)
        {
            RedirectUrl = string.Empty;
            LogoutToken = string.Empty;
            VerifyToken = string.Empty;
            IsNextStepCalled = false;

            var request = new MockHttpRequest(new Dictionary<string, string>() { { "access_token", accessToken } }, pathGetter);
            var response = new MockHttpResponse(url => RedirectUrl = url);
            _mockHttpContext = new MockHttpContext(request, response);

            Func<string, Task<bool>> OnLogout = lt => { LogoutToken = lt; return Task.FromResult(logoutResult); };
            Func<string, Task<bool>> OnVerify = vt => { VerifyToken = vt; return Task.FromResult(verifyResult); };

            var mockAuthService = new Mock<IAuthenticationService>();
            mockAuthService.Setup(s => s.Logout(It.IsAny<string>())).Returns(OnLogout);
            mockAuthService.Setup(s => s.VerifyToken(It.IsAny<string>())).Returns(OnVerify);
            mockAuthService.Setup(s => s.RedirectToLogin(It.IsAny<string>(), It.IsAny<Action<string>>()))
                 .Callback<string, Action<string>>((url, rdr) => rdr(url));

            RequestDelegate reqDlg = context => { IsNextStepCalled = true; return Task.FromResult(true); };
            _middleware = new AuthenticationMiddleware(reqDlg, LoggerFactory, AuthenticationOptions, mockAuthService.Object);
        }

        [Fact]
        public async void LaunchShouldNotVerifyOrLogOutOrRedirect()
        {
            Initialize(null, () => AuthenticationOptions.LaunchPath);
            await _middleware.Invoke(_mockHttpContext);

            //As we use testdoubles test the behaviors by checking the result of the action
            Assert.True(string.IsNullOrEmpty(VerifyToken));
            Assert.True(string.IsNullOrEmpty(LogoutToken));
            Assert.True(string.IsNullOrEmpty(RedirectUrl));

            Assert.True(IsNextStepCalled);
        }

        [Fact]
        public async void InvalidTokenShouldRedirect()
        {
            Initialize(null, () => "/home");
            await _middleware.Invoke(_mockHttpContext);

            Assert.True(string.IsNullOrEmpty(VerifyToken));
            Assert.True(string.IsNullOrEmpty(LogoutToken));
            Assert.True(!string.IsNullOrEmpty(RedirectUrl) && RedirectUrl.EndsWith(AuthenticationOptions.LoginRedirectUrl));
            Assert.False(IsNextStepCalled);

        }


        [Fact]
        public async void ValidTokenShouldBeVerified()
        {
            string accessToken = GetAccessToken();
            Initialize(accessToken, () => "/home");
            await _middleware.Invoke(_mockHttpContext);

            Assert.True(accessToken == VerifyToken);
            Assert.True(string.IsNullOrEmpty(LogoutToken));
            Assert.True(string.IsNullOrEmpty(RedirectUrl));
            Assert.True(IsNextStepCalled);

        }

        [Fact]
        public async void LogoutShouldNotVerifyButRedirect() //verification handled by the api itself
        {
            string accessToken = GetAccessToken();
            Initialize(accessToken, () => AuthenticationOptions.LogoutPath);
            await _middleware.Invoke(_mockHttpContext);

            Assert.True(string.IsNullOrEmpty(VerifyToken));
            Assert.True(accessToken == LogoutToken);
            Assert.True(!string.IsNullOrEmpty(RedirectUrl) && RedirectUrl.EndsWith(AuthenticationOptions.LoginRedirectUrl));
            Assert.False(IsNextStepCalled);

        }

        [Fact]
        public async void FailedTokenVerifyShouldRedirect() 
        {
            string accessToken = GetAccessToken();
            Initialize(accessToken, () => "/home", false, true);
            await _middleware.Invoke(_mockHttpContext);


            Assert.True(accessToken == VerifyToken);
            Assert.True(string.IsNullOrEmpty(LogoutToken));
            Assert.True(!string.IsNullOrEmpty(RedirectUrl) && RedirectUrl.EndsWith(AuthenticationOptions.LoginRedirectUrl));
            Assert.False(IsNextStepCalled);


        }
    }
}
