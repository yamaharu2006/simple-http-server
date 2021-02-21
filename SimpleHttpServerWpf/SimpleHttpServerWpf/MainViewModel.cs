using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimpleHttpServerWpf
{
    class MainViewModel : INotifyPropertyChanged
    {
        public bool ButtonIsEnable { get; private set; }
        public string ButtonContext { get; private set; }
        public DelegateCommand CommandSwitchedServer { get; set; }

        private HttpServer server;

        bool isRunning = false;
        private readonly object objLock = new object();

        public MainViewModel()
        {
            ButtonIsEnable = true;
            ButtonContext = "Run";
            CommandSwitchedServer = new DelegateCommand(x =>  true, ButtonSwitchServer_Click);
        }

        void ButtonSwitchServer_Click(object paramater)
        {
            ButtonIsEnable = false;

            if (!isRunning)
            {
                Start();
            }
            else
            {
                Stop();
            }

            ButtonIsEnable = true;
        }

        void Start()
        {
            server = new HttpServer(new string[] { @"http://localhost:8080/" });

#if true // regist sample response
            var requestRule = new HttpRequestRule();
            var responseHeaders = new NameValueCollection
            {
                { "content-type", "application/json" }
            };
            var responseRule = new HttpResponseRule(HttpStatusCode.OK, responseHeaders, Encoding.UTF8.GetBytes("{}"));
            var rule = new HttpRule(requestRule, responseRule);
            rule.Action = (x) => 
            {
                Console.WriteLine("Callback function called");
            };
            server.AddRule(rule);
#endif
            var isStarted = server.Start();
            UpdateRunning(isStarted);
        }

        void Stop()
        {
            server.Stop();
            UpdateRunning(false);
        }

        void UpdateRunning(bool isRunning)
        {
            if(this.isRunning != isRunning)
            {
                ButtonContext = isRunning != true ? "Run" : "Stop";
                this.isRunning = isRunning;
                NotifyPropertyChanged(nameof(ButtonContext));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
