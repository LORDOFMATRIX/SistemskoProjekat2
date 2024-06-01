using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace Gif_Server
{
    internal class Server
    {
        private static readonly object count_lock = new object();
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        private static int requestCount = 0;


        public Server()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);
        }
        public static async Task HandleRequest(Object item)
        {
                HttpListenerContext context = (HttpListenerContext)item;
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (request.Url.AbsolutePath == "/favicon.ico")
                {
                     return;
                }
                lock (count_lock) { 
                Console.WriteLine("Request broj: {0}", ++requestCount);
                }
                Console.WriteLine(request.Url.ToString());
                Console.WriteLine(request.HttpMethod);
                Console.WriteLine(request.UserHostName);
                Console.WriteLine();

                

                if (request.Url.AbsolutePath.Length < 4 &&
                   (request.Url.AbsolutePath.Length > 5 && request.Url.AbsolutePath.Substring(request.Url.AbsolutePath.Length - 4) != ".gif"))
                {
                    await ReturnBadRequest(response);

                }
                else
                {
                    try
                    {
                        string location = request.Url.AbsolutePath.Substring(1); //path bez '/'
                        byte[] file = null;
                        if ((file = LRUCache.GetFromCache(location))==null)//Cache.ReadFromCache(location)) == null)) 
                        {

                            var files = Directory.EnumerateFiles(@"C:\Users\Branko\source\repos\Gif Server\Gif Server\Slike", location, SearchOption.AllDirectories);
                            file = File.ReadAllBytes(files.First());
                            LRUCache.AddToCache(location, file);
                        }
                        Console.WriteLine("vreme potrebno za čitanje fajla: " + stopwatch.ElapsedMilliseconds + " milisekundi");
                        response.ContentType = "image/gif";
                        response.ContentEncoding = Encoding.ASCII;
                        Console.WriteLine();
                        await response.OutputStream.WriteAsync(file, 0, file.Length);
                        response.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await ReturnBadRequest(response);
                    }
                    finally
                    {
                        stopwatch.Stop();
                        Console.WriteLine("Potrebno vreme je: " + stopwatch.ElapsedMilliseconds + " milisekunde");
                    }


            }
        }
        public static async Task ReturnBadRequest(HttpListenerResponse response)
        {
            byte[] data = Encoding.UTF8.GetBytes("<h1>Nepostojeca slika</h1>");
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            await response.OutputStream.WriteAsync(data, 0, data.Length);
            response.Close();
        }
    }
}
