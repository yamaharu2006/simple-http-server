using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace SimpleHttpServerWpf
{
    class HttpRule
    {
        public Action<HttpListenerRequest> Action { private get; set; }
        public HttpRequestRule RequestRule { get; private set; }
        public HttpResponseRule ResponseRule { get; private set; }

        private int Count { get; set; }

        public HttpRule(HttpRequestRule requestRule, HttpResponseRule responseRule)
        {
            RequestRule = requestRule;
            ResponseRule = responseRule;
            Count = 1;
        }

        public bool IsMatch(HttpListenerRequest request)
        {
            var isMatch = (Count > 0)
                && IsMatchUrl(request.Url)
                && IsMatchMethod(new HttpMethod(request.HttpMethod))
                && IsMatchHeaders(request.Headers);
            
            if(isMatch)
            {
                Action?.Invoke(request);
                Count--;
            }

            return isMatch;
        }

        bool IsMatchUrl(Uri uri)
        {
            if(RequestRule.UrlRegex == null)
            {
                return true;
            }
            return Regex.IsMatch(uri.ToString(), RequestRule.UrlRegex);
        }

        bool IsMatchMethod(HttpMethod method)
        {
            if(RequestRule.Method == null)
            {
                return true;
            }
            return method == RequestRule.Method;
        }

        bool IsMatchHeaders(NameValueCollection headers)
        {
            if(RequestRule.HeadersRegex.Count == 0)
            {
                return true;
            }
            return RequestRule.HeadersRegex.AllKeys.All(x =>
            {
                if (headers[x] != null)
                {
                    if (Regex.IsMatch(headers[x], RequestRule.HeadersRegex[x]))
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        public void OutputStreamResponse(ref HttpListenerResponse response)
        {

            ConstructHeaders(ref response);
            WriteResponse(ref response);
        }

        void ConstructHeaders(ref HttpListenerResponse response)
        {
            foreach (var key in ResponseRule.Headers.AllKeys)
            {
                response.AddHeader(key, ResponseRule.Headers[key]);
            }
        }

        void WriteResponse(ref HttpListenerResponse response)
        {
            var buffer = ResponseRule.Content;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
