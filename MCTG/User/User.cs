using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using MTCG.Http;
using MTCG.DB;

namespace MTCG.User
{
	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string? Bio { get; set; }
		public int Coins { get; set; } = 20;
		public int Won { get; set; } = 0;
		public int Lost { get; set; } = 0;
		public int Elo { get; set; } = 100;
		public int singlePlayerWins { get; set; } = 0;

		public User(string username, string password, int coins, int won, int lost, int elo)
        {
			Username = username;
			Password = password;
			Coins = coins;
			Won = won;
			Lost = lost;
			Elo = elo;

        }

		public void CreateUser(HttpServerEventArgs e)
		{

		}

		public void Login(HttpServerEventArgs e)
        {

        }
	}
}
