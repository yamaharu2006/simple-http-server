using System.Net.Http;
using System.Collections.Specialized;

namespace SimpleHttpServerWpf
{
    class HttpRequestRule
    {
        public string UrlRegex { get; set; }
        public NameValueCollection HeadersRegex { get; set; }
        public HttpMethod Method { get; set; }

        public HttpRequestRule()
        {
            UrlRegex = null;
            HeadersRegex = new NameValueCollection();
            Method = null;
        }

        public HttpRequestRule(string urlRegex, HttpMethod method)
        {
            UrlRegex = urlRegex;
            HeadersRegex = new NameValueCollection();
            Method = method;
        }

    }
}
