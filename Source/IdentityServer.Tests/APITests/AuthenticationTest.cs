using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using IdentityServer.Core.Providers;
using AuthenticaitonAPI.Controllers;
using Microsoft.Extensions.Logging;
using IdentityServer.API;
using IdentityServer.API.Response;
using IdentityServer.Tests.TestDoubles;

namespace IdentityServer.Tests.APITests
{
    public class AuthenticationTest
    {
        const string TestUser = "abdul";
        const string TestPassword = "abdul";

        static readonly IAuthenticationProvider AuthenticationProvider = new StaticUsersAuthProvider();
        static readonly ILoggerFactory LoggerFactory = new LoggerFactoryTestDouble<AuthenticationController>();

        AuthenticationController _controller = null;

        public void Initialize()
        {
            _controller = new AuthenticationController(AuthenticationProvider, LoggerFactory);
        }


        [Fact]
        public async void LoginShouldSucceedOnRightCredentials()
        {
            Initialize();

            var loginResponse = await LoginUser();

            Assert.True(loginResponse.IsSuccess);
            Assert.NotNull(loginResponse.Token);

            var savedResponse = Storage.IssuedTokens.SingleOrDefault(t => t.Token == loginResponse.Token);
            Assert.NotNull(savedResponse);
        }


        [Fact]
        public async void LoginShouldFailOnBadCredentials()
        {
            Initialize();

            var loginResponse = await _controller.Login(new API.Request.LoginRequest { Username = "BAD_USER", Password = TestPassword });
            Assert.False(loginResponse.IsSuccess);
        }

        [Fact]
        public async void LoginShouldFailOnNullRequest()
        {
            Initialize();

            var loginResponse = await _controller.Login(null);
            Assert.False(loginResponse.IsSuccess);

            var loginResponse2 = await _controller.Login(new API.Request.LoginRequest { Username = null, Password = null });
            Assert.False(loginResponse2.IsSuccess);
        }

        [Fact]
        public async void TokenShouldBeValidOnVerifyAfterLogin()
        {
            Initialize();

            var loginResponse = await LoginUser();
            var verifyResponse = _controller.Verify(new API.Request.VerifyRequest { Token = loginResponse.Token });
            Assert.True(verifyResponse.IsSuccess);
        }

        [Fact]
        public void TokenShouldBeInvalidOnBadToken()
        {
            Initialize();

            var verifyResponse = _controller.Verify(new API.Request.VerifyRequest { Token = "BAD_TOKEN" });
            Assert.False(verifyResponse.IsSuccess);
        }

        [Fact]
        public void VerifyShouldFailOnNullRequest()
        {
            Initialize();

            var verifyResponse = _controller.Verify(null);
            Assert.False(verifyResponse.IsSuccess);

            var verifyResponse2 = _controller.Verify(new API.Request.VerifyRequest { Token = string.Empty });
            Assert.False(verifyResponse2.IsSuccess);
        }

        [Fact]
        public async void LogoutShouldRemoveToken()
        {
            Initialize();

            var loginResponse = await LoginUser();
            var logoutResponse = _controller.Logout(new API.Request.LogoutRequest { Token = loginResponse.Token });
            var savedResponse = Storage.IssuedTokens.SingleOrDefault(t => t.Token == loginResponse.Token);
            Assert.Null(savedResponse);
        }

        [Fact]
        public void LogoutShouldFailOnNullRequest()
        {
            Initialize();

            var logoutResponse = _controller.Logout(null);
            Assert.False(logoutResponse.IsSuccess);

            var logoutResponse2 = _controller.Logout(new API.Request.LogoutRequest { Token = string.Empty });
            Assert.False(logoutResponse2.IsSuccess);
        }

        public async Task<LoginResponse> LoginUser()
        {
            if (_controller == null) Initialize();

            return await _controller.Login(new API.Request.LoginRequest { Username = TestUser, Password = TestPassword });
        }

    }


}
