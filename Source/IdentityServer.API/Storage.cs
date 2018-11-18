using IdentityServer.API.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.API
{
    public static class Storage
    {
        public static ConcurrentBag<LoginResponse> IssuedTokens = new ConcurrentBag<LoginResponse>();
    }
}
