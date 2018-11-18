using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Core.Model;

namespace IdentityServer.Core.Providers
{
    /// <summary>
    /// Fake Provider to test against static users
    ///  Username = "abdul", Password = "abdul" 
    ///  Username = "crossover", Password = "crossover"
    ///  Username = "saunders", Password = "saunders"
    /// </summary>
    public class StaticUsersAuthProvider : IAuthenticationProvider
    {
        private static List<Credentials> TestCredentials = new List<Credentials>()
        {
             new Credentials { Username = "abdul", Password = "abdul" },
             new Credentials { Username = "crossover", Password = "crossover" },
             new Credentials { Username = "saunders", Password = "saunders" },
        };

        public AuthenticationResult AuthenticateUser(Credentials credentials)
        {
            AuthenticationResult result = new AuthenticationResult { IsSuccess = false };

            if (credentials != null)
            {
                if (TestCredentials.Any(tc => tc.Username == credentials.Username && tc.Password == credentials.Password))
                {
                    result.IsSuccess = true;
                    result.UserProfile = new UserProfile();
                    result.UserProfile.Name = credentials.Username;
                }
            }
            return result;
        }
    }
}
