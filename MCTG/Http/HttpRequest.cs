using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MTCG.Http.Enums;

namespace MTCG.Http
{
    class HttpRequest
    {
        //constructors
        private TcpClient _client;
        private StreamReader reader;
        public HttpMethod Method { get; private set; }
        public string Path { get; private set; }
        public string HttpVersion { get; private set; }
        public string Body { get; private set; }
        public string PathParameter { get; private set; }
        public string Payload { get; private set; }

        //for mapping
        public Dictionary<string, string> Headers;
        public Dictionary<string, string> EndpointParameters;

        public HttpRequest(TcpClient client)
        {
            _client = client;
            reader = new(client.GetStream());
            Headers = new();
            EndpointParameters = new();
            Method = HttpMethod.Get;
            Path = "";
            HttpVersion = "";
            Payload = null;

        }
    }
}
