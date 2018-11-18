using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.API
{
    /// <summary>
    /// Marker to specify all business exceptions
    /// </summary>
    public class APIException : Exception
    {
        public APIException(string message) : base(message)
        {

        }
    }
}
