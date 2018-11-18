using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Core.Model;
using Novell.Directory.Ldap;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Core.Providers
{
    /// <summary>
    /// Provider implementation for LDAP
    /// </summary>
    public class LDAPAuthenticationProvider : IAuthenticationProvider
    {
        private readonly LDAPConnectionInfo _LDAPConnectionInfo = null;
        private readonly ILogger _logger = null;

        public LDAPAuthenticationProvider(LDAPConnectionInfo @LDAPConnectionInfo, ILoggerFactory loggerFactory)
        {
            _LDAPConnectionInfo = @LDAPConnectionInfo;
            _logger = loggerFactory.CreateLogger<LDAPAuthenticationProvider>();
        }

        public AuthenticationResult AuthenticateUser(Credentials userInfo)
        {
            AuthenticationResult result = new AuthenticationResult();
            try
            {
                LdapConnection conn = new LdapConnection();
                conn.Connect(_LDAPConnectionInfo.Host, _LDAPConnectionInfo.Port);
                conn.Bind(userInfo.Username, userInfo.Password);
                string profileName = null;
                try
                {
                    LdapSearchResults lsc = conn.Search(string.Empty, LdapConnection.SCOPE_SUB, "cn = " + userInfo.Username, null, false);
                    while (lsc.hasMore())
                    {
                        LdapEntry nextEntry = null;
                        try
                        {
                            nextEntry = lsc.next();
                        }
                        catch (LdapException)
                        {
                            continue;
                        }

                        LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
                        LdapAttribute attribute = attributeSet.Cast<LdapAttribute>().Where(att => att.Name.Trim().ToUpper() == "PROFILE_NAME").SingleOrDefault();
                        if (attribute != null)
                            profileName = attribute.StringValue;
                        break;
                    }
                }
                catch { }
                conn.Disconnect();
                result.IsSuccess = true;
                result.UserProfile = new UserProfile();
                result.UserProfile.Name = (profileName == null ? userInfo.Username : profileName);

            }
            catch (LdapException ex)
            {
                _logger.LogCritical(EventIds.AuthenticateUser, ex, nameof(LdapException));
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(EventIds.AuthenticateUser, ex, nameof(Exception));
                result.ErrorMessage = ex.Message;
                result.IsServerError = true;
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
