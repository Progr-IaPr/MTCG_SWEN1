using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTCG.Models
{
	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public int Coins { get; set; } = 20;
		public string? Name { get; set; }
		public string? Bio { get; set; }
		public string? Image { get; set; }
		public string Token { get; set; }

		public User(string username, string password, int coins, string name, string bio, string image, string token)
		{
			Username = username;
			Password = password;
			Coins = coins;
			Name = name;
			Bio = bio;
			Image = image;
			Token = token;

		}

	}
}
