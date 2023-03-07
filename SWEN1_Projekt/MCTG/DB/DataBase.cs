using System;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MTCG.Http;
using MTCG.Models;

namespace MTCG.DB
{
	public class DataBase
	{
		//string for db connection
		public NpgsqlConnection? connection;
		public NpgsqlCommand? cmd;
		public static string connString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcg_db";
	

		//here we're trying to connect to our db
		public NpgsqlConnection OpenConnection()
        {
			try
            {
				connection = new NpgsqlConnection(connString);
				connection.Open();

            }
			catch (Exception ex)
            {
				Console.WriteLine("Error: " +ex.Message, "Couldn't connect to database!!");
				connection.Close();
            }
			return connection;
        }

		//and here we're disconnecting from our db
		public void CloseConnection()
        {
			connection.Close();
        }

		//is being used before we want to run the curl script to make sure it's succeeding
		public void DeleteAll()
		{
			//connecting to db
			OpenConnection();

			//this sql command deletes all data from all tables of the db
			using (cmd = new NpgsqlCommand("DO $$ DECLARE r RECORD; BEGIN FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE;'; END LOOP; END $$;", connection))
			{
				cmd.ExecuteNonQuery();
			}
			CloseConnection();
		}

		//////////////////////////////////////////////////////////////////////////////
		///				Here are our methods regarding Users            		   //
		//////////////////////////////////////////////////////////////////////////////

		//adding a new user to db
		public void CreateUser(HttpServerEventArgs e)
        {
			try
			{
				//extracting username & password + constructing token for future authentification
				User? user = JsonSerializer.Deserialize<User>(e.Payload);
				string token = "Basic " + user.Username + "-mtcgToken";

				//hashing the given password
				string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, 12);

				if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
				{
					e.Reply(400, "An Error occured while trying to create the user!");
					return;
				}

				//connect to db
				OpenConnection();

				//selecting users from db that have the same username to check whether user already exists
				using (cmd = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM users WHERE username = @username)", connection))
				{
					cmd.Parameters.AddWithValue("username", user.Username);

					bool userExists = (bool)cmd.ExecuteScalar();

					//replying with an error if a username was found
					if (userExists)
					{
						e.Reply(400, "Error: Username already exists!");
						return;
					}

				}

				using (cmd = new NpgsqlCommand("INSERT INTO users (username, password, coins, token) VALUES (@username, @password, @coins, @token)", connection))
				{
					cmd.Parameters.AddWithValue("username", user.Username);
					cmd.Parameters.AddWithValue("password", hashedPassword);
					cmd.Parameters.AddWithValue("coins", 20);
					cmd.Parameters.AddWithValue("token", token);
					cmd.ExecuteNonQuery();
				}
				InsertStats(e);
				CloseConnection();
			}
            catch
            {
				e.Reply(400, "Error: Couldn't create User!");
            }
		}
		

		//here we're retrieving username & password for login
		public void LoginUser(HttpServerEventArgs e)
        {
			try
            {
				User? user = JsonSerializer.Deserialize<User>(e.Payload);
				string? hashedPassword = null;

				OpenConnection();

				using (cmd = new NpgsqlCommand("SELECT password FROM users WHERE username = @username", connection))
				{
					cmd.Parameters.AddWithValue("username", user.Username);
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
						while (reader.Read())
						{
							hashedPassword = reader.GetString(0);
						}
					}
				}

				//here we're checking whether the given password and the hashed password in the db match
				bool isMatch = BCrypt.Net.BCrypt.Verify(user.Password, hashedPassword);
				if (isMatch)
                {
					e.Reply(200, "User was logged in successfully.");
                }
                else
                {
					e.Reply(400, "Error: Couldn't login user!");
                }
				CloseConnection();
			}
			catch
            {
				e.Reply(400, "Unexpected Error occured while trying to login user!");
            }	
        }

		//for registration purposes..sets stats values in db
		public void InsertStats(HttpServerEventArgs e)
        {
			try
			{


				User? user = JsonSerializer.Deserialize<User>(e.Payload);
				OpenConnection();

				using (cmd = new NpgsqlCommand("INSERT INTO stats (username, games, wins, losses, draws, elo) VALUES (@username, @games, @wins, @losses, @draws, @elo)", connection))
				{
					cmd.Parameters.AddWithValue("username", user.Username);
					cmd.Parameters.AddWithValue("games", 0);
					cmd.Parameters.AddWithValue("wins", 0);
					cmd.Parameters.AddWithValue("losses", 0);
					cmd.Parameters.AddWithValue("draws", 0);
					cmd.Parameters.AddWithValue("elo", 100);
					cmd.ExecuteNonQuery();
				}
				CloseConnection();
				e.Reply(200, "User was created and stats were added successfully");
			}
            catch
            {
				e.Reply(400, "Unexpected Error occured while trying to insert stats!");
            }
			
		}

		//check if user's logged in
		public string UserAuthentification(HttpServerEventArgs e)
        {
			try
			{
				Authentification auth = new Authentification();

				//here wer're extracting the token
				int index = 0;
				for (int i = 0; i < e.Headers.Length; i++)
				{
					if (e.Headers[i].Name == "Authorization")
					{
						index = i;
						break;
					}
				}
				string authHeader = e.Headers[index].Value;

				//some helper variables (also we need to return userString later on)
				bool userExists;
				string? userString = "";

				//extracting username from token
				string[] splitString = authHeader.Split(' ');
				string userGivenString = splitString[1].Split('-')[0];

				//connecting to db
				OpenConnection();

				//checking whether token exists in db
				using (cmd = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM users WHERE token = @token)", connection))
				{
					cmd.Parameters.AddWithValue("token", authHeader);

					userExists = (bool)cmd.ExecuteScalar();

					//replying with an error if token wasn't found
					if (!userExists)
					{
						e.Reply(400, "Error: User doesn't exist!");
						return null;
					}
					else
					{
						//getting the username from the table with the token to match it later with the user-given name
						using (cmd = new NpgsqlCommand("SELECT username FROM users WHERE token = @token", connection))
						{
							cmd.Parameters.AddWithValue("token", authHeader);
							using (NpgsqlDataReader reader = cmd.ExecuteReader())
							{
								while (reader.Read())
								{
									auth.UserString = reader.GetString(0);
								}
							}
						}

						CloseConnection();

						if (auth.UserString == userGivenString)
						{
							return auth.UserString;
						}
					}

					return null;
				}
			}
            catch
            {
				e.Reply(400, "Error: No token given!");
				return null;
            }
            	
		}

		public bool CheckIfUserIsAdmin(string userString)
        {
			if(userString == "admin")
            {
				return true;
            }
			return false;
        }

		public void GetUserData(HttpServerEventArgs e, string userString)
        {
			try
			{


				string[] user = e.Path.Split("/");
				string userInfo = "Your Profile Page: \n";

				if (user[2] == userString)
				{
					OpenConnection();
					using (cmd = new NpgsqlCommand("SELECT username, coins, bio, image , name FROM users WHERE username = @username", connection))
					{
						cmd.Parameters.AddWithValue("username", userString);
						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								userInfo += "Username: " + reader.GetString(0) + "\nCoins: " + "\nName: " + reader.GetString(4) + reader.GetInt64(1) + "\nBio: " + reader.GetString(2) + "\nImage: " + reader.GetString(3);
							}
						}
					}
					CloseConnection();
					e.Reply(200, userInfo);
				}
				else
				{
					e.Reply(400, "Something went wrong with the Authorization!");
				}
			}
			catch
            {
				e.Reply(400, "Unexpected Error occured while trying to retrieve User Data!");
            }
        }

		public void EditUserData(HttpServerEventArgs e, string userString)
        {
			try
			{
				string[] user = e.Path.Split("/");

				if (user[2] == userString)
				{
					User? newuserInfo = JsonSerializer.Deserialize<User>(e.Payload);
					OpenConnection();
					using (cmd = new NpgsqlCommand("UPDATE users SET name = @name, bio = @bio, image = @image WHERE username = @username", connection))
					{
						cmd.Parameters.AddWithValue("name", newuserInfo.Name);
						cmd.Parameters.AddWithValue("bio", newuserInfo.Bio);
						cmd.Parameters.AddWithValue("image", newuserInfo.Image);
						cmd.Parameters.AddWithValue("username", userString);
						cmd.ExecuteNonQuery();
					}
					CloseConnection();
					e.Reply(200, "User Data updated successfully!");
				}
				else
				{
					e.Reply(400, "Something went wrong with the Authorization!");
				}
			}
            catch
            {
				e.Reply(400, "Unexpected Error occured while trying to edit User Data!");
			}
		}

		public void GetUserStats(HttpServerEventArgs e, string userString)
        {
			try
			{
				OpenConnection();
				string stats = "Your stats: \n";

				using (cmd = new NpgsqlCommand("SELECT games, wins, losses, draws, elo FROM stats WHERE username = @username", connection))
				{
					cmd.Parameters.AddWithValue("username", userString);

					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							stats += "Username: " + userString + "\nGames played: " + reader.GetInt64(0) + "\nGames won: " + reader.GetInt64(1) + "\nGames lost: " + reader.GetInt64(2) + "\nDraws: " + reader.GetInt64(3) + "\nELO: " + reader.GetInt64(4);

						}
					}
				}
				e.Reply(200, stats);
			}
			catch
            {
				e.Reply(400, "Unexpected Error occured while trying to get User Stats!");
			}
        }

		public void GetScoreboard(HttpServerEventArgs e, string userString)
        {
			try
			{
				OpenConnection();
				string scoreboard = "Scoreboard: \n";
				using (cmd = new NpgsqlCommand("SELECT username, elo FROM stats ORDER BY elo DESC", connection))
				{
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							scoreboard += "\nUsername: " + reader.GetString(0) + "\nELO: " + (reader.GetInt64(1));
						}
					}
				}
				CloseConnection();
				e.Reply(200, scoreboard);
			}
            catch
            {
				e.Reply(400, "Unexpected Error occured while trying to get Scoreboard!");
			}
        }

		/////////////////////////////////////////////////////////////////////////////////////////
		///						Here are all methods regarding cards						 ///
		/////////////////////////////////////////////////////////////////////////////////////////

		public void CreateCards(HttpServerEventArgs e)
        {
			try
			{
				List<Card> package = JsonSerializer.Deserialize<List<Card>>(e.Payload);

				OpenConnection();

				if (package.Count != 5)
				{
					e.Reply(400, "Error: In order to create a package 5 cards are needed!");
					return;
				}

				// those variables we're using to pass the id of the cards onto the package table later
				string[] cardIds = new string[5];
				int i = 0;

				foreach (Card card in package)
				{
					//at first we need to determine the type and element of the card by checking the cardname
					//not the most beautiful implementation but it works
					if (card.Name.Contains("Spell"))
					{
						card.Type = (int)Card.CardType.Spell;
					}
					else if (card.Name.Contains("Goblin"))
					{
						card.Type = (int)Card.CardType.Goblin;
					}
					else if (card.Name.Contains("Dragon"))
					{
						card.Type = (int)Card.CardType.Dragon;
					}
					else if (card.Name.Contains("Wizzard"))
					{
						card.Type = (int)Card.CardType.Wizzard;
					}
					else if (card.Name.Contains("Ork"))
					{
						card.Type = (int)Card.CardType.Ork;
					}
					else if (card.Name.Contains("Knight"))
					{
						card.Type = (int)Card.CardType.Knight;
					}
					else if (card.Name.Contains("Kraken"))
					{
						card.Type = (int)Card.CardType.Kraken;
					}
					else if (card.Name.Contains("Elf"))
					{
						card.Type = (int)Card.CardType.Elf;
					}
					else if (card.Name.Contains("Troll"))
					{
						card.Type = (int)Card.CardType.Troll;
					}

					if (card.Name.Contains("Water"))
					{
						card.Element = (int)Card.CardElement.Water;
					}
					else if (card.Name.Contains("Fire"))
					{
						card.Element = (int)Card.CardElement.Fire;
					}
					else
					{
						card.Element = (int)Card.CardElement.Regular;
					}

					//after we're done determining everything we can insert the information into the db..first into the cards table
					using (cmd = new NpgsqlCommand("INSERT INTO cards (id, name, element, type, damage, deck) VALUES (@id, @name, @element, @type, @damage, @deck)", connection))
					{
						cmd.Parameters.AddWithValue("id", card.Id);
						cmd.Parameters.AddWithValue("name", card.Name);
						cmd.Parameters.AddWithValue("element", card.Element);
						cmd.Parameters.AddWithValue("type", card.Type);
						cmd.Parameters.AddWithValue("damage", card.Damage);
						cmd.Parameters.AddWithValue("deck", false);
						cmd.ExecuteNonQuery();
					}
					cardIds[i] = card.Id;
					i++;
				}
				using (cmd = new NpgsqlCommand("INSERT INTO packages (cardid0, cardid1, cardid2, cardid3, cardid4) VALUES (@cardid0, @cardid1, @cardid2, @cardid3, @cardid4)", connection))
				{
					cmd.Parameters.AddWithValue("cardid0", cardIds[0]);
					cmd.Parameters.AddWithValue("cardid1", cardIds[1]);
					cmd.Parameters.AddWithValue("cardid2", cardIds[2]);
					cmd.Parameters.AddWithValue("cardid3", cardIds[3]);
					cmd.Parameters.AddWithValue("cardid4", cardIds[4]);
					cmd.ExecuteNonQuery();
				}
				CloseConnection();
				e.Reply(200, "Packages were created successfully!");
			}
			catch
            {
				e.Reply(400, "Unexpected Error occured while trying to create Packages!");
			}
        }

		public void AquirePackage(HttpServerEventArgs e, string userString)
        {
			try
			{
				OpenConnection();

				//some variables we need
				int coins = 0;
				string[] cardID = new string[5];
				string username = userString.ToString();

				//checking whether the user has enough money
				using (cmd = new NpgsqlCommand("SELECT coins FROM users WHERE username = @username", connection))
				{
					cmd.Parameters.AddWithValue("username", username);
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							coins = reader.GetInt32(0);
						}
					}
				}
				if (coins < 5)
				{
					e.Reply(400, "Error: You need at least 5 coins to aquire a package!");
					return;
				}

				//retrieving package from db
				using (cmd = new NpgsqlCommand("SELECT cardid0, cardid1, cardid2, cardid3, cardid4 FROM packages ORDER BY id ASC LIMIT 1", connection))
				{
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							cardID[0] = reader.GetString(0);
							cardID[1] = reader.GetString(1);
							cardID[2] = reader.GetString(2);
							cardID[3] = reader.GetString(3);
							cardID[4] = reader.GetString(4);
						}
					}
				}
				if (string.IsNullOrEmpty(cardID[0]))
				{
					e.Reply(400, "Error: No packages available!");
					return;
				}

				//getting through cardid array and adding username to cards in db
				foreach (string cardid in cardID)
				{
					using (cmd = new NpgsqlCommand("UPDATE cards SET username = @username WHERE id = @cardid", connection))
					{
						cmd.Parameters.AddWithValue("username", username);
						cmd.Parameters.AddWithValue("cardid", cardid);
						cmd.ExecuteNonQuery();
					}
				}

				//updating coins in user table
				using (cmd = new NpgsqlCommand("UPDATE users SET coins = @coins WHERE username = @username", connection))
				{
					cmd.Parameters.AddWithValue("coins", coins - 5);
					cmd.Parameters.AddWithValue("username", userString);
					cmd.ExecuteNonQuery();
				}

				//deleting the package from table since it's already been bought
				using (cmd = new NpgsqlCommand("DELETE FROM packages WHERE cardid0 = @cardid", connection))
				{
					cmd.Parameters.AddWithValue("cardid", cardID[0]);
					cmd.ExecuteNonQuery();
				}
				CloseConnection();

				e.Reply(200, "Package aquired successfully!");
			}
			catch
            {
				e.Reply(400, "Unexpected Error occured while trying to aquire the package!");
			}
        }

		public void GetCards (HttpServerEventArgs e, string userString)
        {
			try
			{
				OpenConnection();
				string usersCards = "All your cards: \n";

				using (cmd = new NpgsqlCommand("SELECT name FROM cards WHERE username = @username", connection))
				{
					cmd.Parameters.AddWithValue("username", userString);

					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							usersCards += "\n" + reader.GetString(0) + "\n";
						}
					}
				}
				if (usersCards == "All your cards: \n")
				{
					e.Reply(200, "You don't have any cards yet!");
				}
				CloseConnection();
				e.Reply(200, usersCards);
			}
			catch
            {
				e.Reply(400, "Unexpected Error occured while trying to retrieve all the cards!");
			}
        }

		public void GetDeck(HttpServerEventArgs e, string userString)
        {
			try
			{
				OpenConnection();
				string usersDeck = "Your Deck: \n";

				using (cmd = new NpgsqlCommand("SELECT name FROM cards WHERE username = @username AND deck = TRUE", connection))
				{
					cmd.Parameters.AddWithValue("username", userString);
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							usersDeck += "\n" + reader.GetString(0) + "\n";
						}
					}
				}
				CloseConnection();
				if (usersDeck == "Your Deck: \n")
				{
					e.Reply(200, "You haven't configured a deck yet!");
					return;
				}
				e.Reply(200, usersDeck);
			}
			catch
            {
				e.Reply(400, "Unexpected Error occured while trying to get User's Deck!");
			}
        }

		public void ConfigureDeck(HttpServerEventArgs e, string userString)
        {
			try
			{
				OpenConnection();
				string[]? deck = JsonSerializer.Deserialize<string[]>(e.Payload);
				int cards = 0;

				if (deck.Length != 4)
				{
					e.Reply(400, "Error: You need to pick 4 cards to configure your deck successfully");
					return;
				}

				//checking if user is using only his own cards
				using (cmd = new NpgsqlCommand("SELECT count (*) FROM cards WHERE username = @username AND (id = @id1 OR id = @id2 OR id = @id3 OR id = @id4)", connection))
				{
					cmd.Parameters.AddWithValue("username", userString);
					cmd.Parameters.AddWithValue("id1", deck[0]);
					cmd.Parameters.AddWithValue("id2", deck[1]);
					cmd.Parameters.AddWithValue("id3", deck[2]);
					cmd.Parameters.AddWithValue("id4", deck[3]);

					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							cards = reader.GetInt32(0);
						}
					}
				}
				if (cards < 4)
				{
					e.Reply(400, "Error: You can only pick cards from your own stack!");
					return;
				}

				//before updating the deck we need to reset it first
				using (cmd = new NpgsqlCommand("UPDATE cards SET deck = false WHERE username = @username", connection))
				{
					cmd.Parameters.AddWithValue("username", userString);
					cmd.ExecuteNonQuery();
				}

				//now we can update the deck 
				using (cmd = new NpgsqlCommand("UPDATE cards SET deck = true WHERE username = @username AND (id = @id1 OR id = @id2 OR id = @id3 OR id = @id4)", connection))
				{
					cmd.Parameters.AddWithValue("username", userString);
					cmd.Parameters.AddWithValue("id1", deck[0]);
					cmd.Parameters.AddWithValue("id2", deck[1]);
					cmd.Parameters.AddWithValue("id3", deck[2]);
					cmd.Parameters.AddWithValue("id4", deck[3]);
					cmd.ExecuteNonQuery();
				}
				CloseConnection();
				e.Reply(200, "Deck successfully configured!");
			}
            catch
            {
				e.Reply(400, "Error: Couldn't configure deck!");
			}
        }
	}
}
