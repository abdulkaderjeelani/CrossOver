using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.API.Response
{
    public class LoginResponse : IdentityServerResponse
    {
        /// <summary>
        /// access_token
        /// </summary>

        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; } = null;

        /// <summary>
        /// expires_in
        /// </summary>
        [JsonProperty(PropertyName = "expires")]
        public DateTime Expires { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
