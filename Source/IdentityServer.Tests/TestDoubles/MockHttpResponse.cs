using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace IdentityServer.Tests.TestDoubles
{
    public class MockHttpResponse : HttpResponse
    {
        Action<string> _redirector = null;

        public MockHttpResponse(Action<string> redirector)
        {
            _redirector = redirector;
        }

        public override void Redirect(string location, bool permanent)
        {
            _redirector(location);
        }

        #region Not Used
        public override Stream Body
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override long? ContentLength
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override IResponseCookies Cookies
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool HasStarted
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IHeaderDictionary Headers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override HttpContext HttpContext
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int StatusCode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

#endregion
    }
}
