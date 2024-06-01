using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Gif_Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            new Server();

            while (Server.listener.IsListening)
            {
                HttpListenerContext context = await Server.listener.GetContextAsync();
                _ = Task.Run(() => Server.HandleRequest(context));
            }
            Server.listener.Close();


        }

        

    }
}
