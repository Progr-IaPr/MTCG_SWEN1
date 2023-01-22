using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using MTCG.Http;
using MTCG.User;

namespace MTCG
{  
    public static class Program
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // main entry point                                                                                         //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Entry point.</summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {

            HttpServer server = new HttpServer();
            server.Incoming += _Server_Incoming;

            server.Run();
        }
        

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event handlers                                                                                           //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Processes an incoming HTTP request.</summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        public static void _Server_Incoming(object sender, HttpServerEventArgs e)
        {
            
        }
    }
    
}
