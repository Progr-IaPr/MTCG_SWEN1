using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using MTCG.Http;
using MTCG.DB;

namespace MTCG
{  
    public class Program
    {
        static void Main(string[] args)
        {
          DataBase db = new DataBase();   
          Console.WriteLine("Welcome to our Monster Trading Card Game");

          db.DeleteAll();
          Console.WriteLine("The database is now empty and ready to be filled by the Curl Script.");
          HttpServer server = new HttpServer();
          Console.WriteLine("Started server");
          server.Incoming += _Svr_Incoming;

          server.Run();
        }

        /// <summary>Processes an incoming HTTP request.</summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        public static void _Svr_Incoming(object sender, HttpServerEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(HttpRequestHandler._Svr_Incoming, e);
        }
    }  
}
