using System;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DB
{
	public class DataBaseConnection
	{
		private static string host = "localhost";
		private static string user = "postgres";
		private static string password = "postgres";
		private static string db = "mtcg";
		private static int port = 5432;

		private string connection;
		
		public DataBaseConnection()
        {
			connection = $"Server={host};Username={user};Password={password};Database={db};";
			
		}

		public NpgsqlConnection Connect()
        {
			var DBConn = new NpgsqlConnection(connection);
			return DBConn;
        }

	}
}
