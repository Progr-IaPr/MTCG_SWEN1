using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MTCG.Http
{
	public class HttpResponse
	{
		private TcpClient _client;

		public string StatusMessage { get; set; }
		public string Version { get; set; }
		public string Body { get; set; }
		public Dictionary<string, string> Headers { get; set; }

		public HttpResponse(TcpClient client)
        {
			_client = client;
			Headers = new();
        }
	}
}
