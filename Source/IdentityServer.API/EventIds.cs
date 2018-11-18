using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.API
{
    public static class EventIds
    {
        public static EventId Login = new EventId(1, "LOGIN");
        public static EventId Logout = new EventId(2, "LOGOUT");
        public static EventId Verify = new EventId(3, "VERIFY");
    }
}
