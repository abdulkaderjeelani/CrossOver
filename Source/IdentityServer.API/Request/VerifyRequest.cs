﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.API.Request
{
    public class VerifyRequest
    {
        public string Token { get; set; }
        public string ClientIp { get; set; }
    }
}
