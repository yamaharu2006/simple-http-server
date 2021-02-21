using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SimpleHttpServerWpf
{
    class HttpServer
    {
        private HttpListener listener;
        private List<HttpRule> rules;

        public HttpServer(string[] prefixes)
        {
            listener = new HttpListener();
            rules = new List<HttpRule>();
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
        }

        public bool Start()
        {
            lock(listener)
            {
                if (!CanStart())
                {
                    return false;
                }

                listener.Start();
                Console.WriteLine("Listening...");
                IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
                return true;
            }
        }

        bool CanStart()
        {
            if (listener.IsListening)
            {
                Console.WriteLine("Already a server is Listening");
                return false;
            }
            else if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return false;
            }
            else if (listener.Prefixes.Count == 0)
            {
                Console.WriteLine("Not set prefixes");
                return false;
            }
            return true;
        }

        void ListenerCallback(IAsyncResult result)
        {
            lock (listener)
            {
                try
                {
                    HttpListener listener = (HttpListener)result.AsyncState;

                    Console.WriteLine("Call EndGetContext to complete the asynchronous operation.");
                    HttpListenerContext context = listener.EndGetContext(result);

                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    rules.First(x => x.IsMatch(request))?.OutputStreamResponse(ref response);
                }
                catch (HttpListenerException e)
                {
                    Console.WriteLine("Stopped Server while EndGetContext()");
                    Console.WriteLine(e.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void Stop()
        {
            lock (listener)
            {
                Console.WriteLine("Stop server");
                listener.Stop();
            }
        }

        public void AddRule(HttpRule rule)
        {
            if (!listener.IsListening)
            {
                rules.Add(rule);
            }
        }

        public void RemoveRule(HttpRule rule)
        {
            if (!listener.IsListening)
            {
                rules.Remove(rule);
            }
        }

        public void ClearRules()
        {
            if (!listener.IsListening)
            {
                rules.Clear();
            }
        }
    }
}
