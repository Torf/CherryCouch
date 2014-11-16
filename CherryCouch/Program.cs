using System;
using System.Text.RegularExpressions;
using System.Threading;
using CherryCouch.Core;
using CherryCouch.Core.Providers;
using Nancy.Hosting.Self;
using NLog;

namespace CherryCouch
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            string ip = Global.Instance.Ip;
            short port = Global.Instance.Port;

            var ipRegex = new Regex(@"^(\d{1,3}\.){3}(\d{1,3})$");
            
            if (!ipRegex.IsMatch(ip))
            {
                Logger.Error("error : invalid ip");
                return;
            }
            
            string url = String.Format("http://{0}:{1}/", ip, port);
            
            Logger.Info("Loading CherryCouch...\n");

            ProvidersManager.Initialize();

            Logger.Info("CherryCouch Loaded.\n\n");
            Logger.Info("Starting front end...");

            using (var nancyHost = new NancyHost(new Uri(url)))
            {
                nancyHost.Start();
                Logger.Info("Front end started.");
                Logger.Info("CherryCouch now listening on " + url + ".\n\n");

                do
                {
                    Logger.Info("Insert \"exit\" to stop CherryCouch.");
                    Thread.Sleep(10);
                } while (Console.ReadLine() != "exit");
            }

            ProvidersManager.Dispose();

            Logger.Info("Stopped. Good bye!");
        }
    }
}
