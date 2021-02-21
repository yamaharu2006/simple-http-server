using System;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace SimpleHttpServerWpf
{
    class HttpResponseRule
    {
        public HttpStatusCode StatusCode { get; set; }
        public NameValueCollection Headers { get; set; }
        public byte[] Content { get; set; }

        private HttpResponseRule()
        {
            Headers = new NameValueCollection();
        }

        public HttpResponseRule(HttpStatusCode statusCode, NameValueCollection headers, byte[] content) : this()
        {
            StatusCode = statusCode;
            Headers = headers;
            Content = content;
        }
    }
}
