using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core
{
    /// <summary>
    /// Event Ids to log for IdentityServer.Core
    /// </summary>
    public static class EventIds
    {
        public static EventId AuthenticateUser = new EventId(100, "AUTHENTICATEUSER");
        
    }
}
