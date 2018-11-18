using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Primitives;
using System.Collections;

namespace IdentityServer.Tests.TestDoubles
{
    public class MockHttpRequest : HttpRequest
    {
        readonly QueryCollection _queryCollection;
        Func<string> _pathGetter;

        public MockHttpRequest(Dictionary<string, string> queryValues, Func<string> pathGetter)
        {
            _queryCollection = new QueryCollection(queryValues);
            _pathGetter = pathGetter;
        }
        
        public override PathString Path
        {
            get
            {
                return new PathString(_pathGetter());
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override IQueryCollection Query
        {
            get
            {
                return _queryCollection;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #region NOT USED

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

        public override IRequestCookieCollection Cookies
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

        public override IFormCollection Form
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

        public override bool HasFormContentType
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

        public override HostString Host
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

        public override HttpContext HttpContext
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsHttps
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

        public override string Method
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



        public override PathString PathBase
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

        public override string Protocol
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



        public override QueryString QueryString
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

        public override string Scheme
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

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class QueryCollection : IQueryCollection
    {
        readonly Dictionary<string, string>  QueryValues;
        public QueryCollection(Dictionary<string, string> queryValues)
        {
            QueryValues = queryValues;
        }

        public StringValues this[string key]
        {
            get
            {
                return new StringValues(QueryValues[key]);
            }
        }

        #region NOT USED

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
