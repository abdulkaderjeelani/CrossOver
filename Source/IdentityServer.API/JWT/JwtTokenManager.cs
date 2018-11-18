using IdentityServer.Core.Model;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.API.JWT
{
    /// <summary>
    /// Issues and verifies token,on JWT standards
    /// </summary>
    public class JwtTokenManager
    {
        private const string SecretKey = "identity_server_secret";
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        private static readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        private static JwtIssuerOptions JWTOptions =>
        new JwtIssuerOptions()
        {
            Issuer = "identity_server",
            Audience = "private_lan",
            NotBefore = DateTime.Now,
            ValidFor = TimeSpan.FromMinutes(60),
            IssuedAt = DateTime.Now,
            SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
        };

        private static TokenValidationParameters TokenValidationParameters =>
         new TokenValidationParameters
         {
             // The signing key must match!
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = SigningKey,

             // Validate the JWT Issuer (iss) claim
             ValidateIssuer = true,
             ValidIssuer = JWTOptions.Issuer,

             // Validate the JWT Audience (aud) claim
             ValidateAudience = true,
             ValidAudience = JWTOptions.Audience,

             // Validate the token expiry
             ValidateLifetime = true,

             // If you want to allow a certain amount of clock drift, set that here:
             ClockSkew = TimeSpan.Zero
         };

        public static async Task<JwtSecurityToken> GetJwtTokenForIdentity(Credentials credentials, ClaimsIdentity identity)
        {
            JwtSecurityToken jwt = null;
            if (identity != null)
            {

                var claims = new[] {new Claim(JwtRegisteredClaimNames.Sub, credentials.Username),
                                        new Claim(JwtRegisteredClaimNames.Jti, await JWTOptions.JtiGenerator()),
                                        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(JWTOptions.IssuedAt).ToString(),ClaimValueTypes.Integer64),
                                        identity.FindFirst("Name")};

                jwt = new JwtSecurityToken(issuer: JWTOptions.Issuer,
                                          audience: JWTOptions.Audience,
                                          claims: claims,
                                          notBefore: JWTOptions.NotBefore,
                                          expires: JWTOptions.Expiration,
                                          signingCredentials: JWTOptions.SigningCredentials);

            }
            return jwt;
        }

        public static bool ValidateToken(string token)
        {
            bool isTokenValid = true;

            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken validToken = null;
            try
            {
                principal = handler.ValidateToken(token, TokenValidationParameters, out validToken);

                var validJwt = validToken as JwtSecurityToken;

                if (validJwt == null)                
                    throw new ArgumentException("Invalid JWT");
                
                if (!validJwt.Header.Alg.Equals(JWTOptions.SigningCredentials.Algorithm, StringComparison.Ordinal))
                    throw new ArgumentException($"Algorithm must be '{JWTOptions.SigningCredentials.Algorithm}'");
                
                //custom validation of JWT claims here (if any)
            }
            catch (SecurityTokenValidationException)
            {
                isTokenValid = false;
            }
            catch (Exception)
            {
                isTokenValid = false;
            }

            return isTokenValid;
        }

        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);


    }
}
