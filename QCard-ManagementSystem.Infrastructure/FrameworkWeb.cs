using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Agit.Framework.Web
{
    //framework 
    public static class HttpContext
    {
        private static IHttpContextAccessor m_httpContextAccessor;
        static IServiceProvider services = null;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                m_httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return m_httpContextAccessor?.HttpContext;
            }
        }

        public static Uri GetAbsoluteUri()
        {
            var request = m_httpContextAccessor.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            //uriBuilder.Port = request.Host.Port.Value;
            uriBuilder.Path = request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();
            return uriBuilder.Uri;
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.Set(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public static class VirtualPathUtility
    {
        public static string ToAbsolute(string contentPath)
        {
            return GenerateClientUrl(contentPath);
        }

        private static string GenerateClientUrl(string path)
        {
            PathString applicationPath = "/";
            if (path.StartsWith("~", StringComparison.Ordinal))
            {
                var segment = new PathString(path.Substring(1));
                return applicationPath.Add(segment).Value;
            } 

            return path;
        }
    }

}