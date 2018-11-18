using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IdentityServer.Core
{
    /// <summary>
    /// Helper class to retrieve the ip of the client. Here to help when TODO Prevent DOS
    /// </summary>
    public static class IPObtainer
    {
        public static string GetRequestIP(HttpContext httpContext, bool tryUseXForwardHeader = true)
        {
            string ip = null;
            try
            {

                if (tryUseXForwardHeader)
                    ip = GetHeaderValueAs<string>(httpContext, "X-Forwarded-For").SplitCsv().FirstOrDefault();

                // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
                if (string.IsNullOrWhiteSpace(ip) && httpContext?.Connection?.RemoteIpAddress != null)
                    ip = httpContext.Connection.RemoteIpAddress.ToString();

                if (string.IsNullOrWhiteSpace(ip))
                    ip = GetHeaderValueAs<string>(httpContext, "REMOTE_ADDR");


                if (string.IsNullOrWhiteSpace(ip))
                    throw new Exception("Unable to determine caller's IP.");
            }
            catch { }
            return ip;
        }

        public static T GetHeaderValueAs<T>(HttpContext httpContext, string headerName)
        {
            StringValues values;

            if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!string.IsNullOrWhiteSpace(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }

        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }
    }
}
