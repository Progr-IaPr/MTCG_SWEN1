namespace MTCG_Test
{
    public class Test
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestEventArgs()
        {
            HttpServerEventArgs e = new HttpServerEventArgs("GET /deck HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nAuthorization: Basic kienboec-mtcgToken\r\nContent-Length: 0", null);

            Assert.That(e.Method, Is.EqualTo("GET"));
            Assert.That(e.Path, Is.EqualTo("/deck"));
        }

        [Test]
        public void TestCreateUser()
        {
            User user = new User("username", "123", 20, "name", "Basic username-mtcgToken");

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Coins, Is.EqualTo(20));
            Assert.That(user.Token, Is.EqualTo("Basic username-mtcgToken"));
            Assert.That(user.Name, Is.EqualTo("name"));
        }

        [Test]
        public void TestCreateUserWithEmptyPassword()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("POST /users HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: *\r\nContent-Type: application/json\r\nContent-Length: 35\r\n\r\n{\"Username\":\"Name1\", \"Password\":\"\"}", null);
            args.SetupGet(m => m.Method).Returns("POST");
            args.SetupGet(m => m.Path).Returns("/users");
            
            db.CreateUser(args.Object);
            args.Verify(e => e.Reply(400, "An Error occured while trying to create the user!"));
        }

        [Test]
        public void TestCreateUserWithEmptyUsername()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("POST /users HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: *\r\nContent-Type: application/json\r\nContent-Length: 35\r\n\r\n{\"Username\":\"\", \"Password\":\"SomePw\"}", null);
            db.CreateUser(args.Object);
            args.Verify(e => e.Reply(400, "An Error occured while trying to create the user!"));
        }

        [Test]
        public void TestLoginUserCorrect()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("POST /sessions HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nContent-Type: application/json\r\nContent-Length: 39\r\n\r\n{\"Username\":\"kienboec\", \"Password\":\"daniel\"}", null);
            db.LoginUser(args.Object);
            args.Verify(e => e.Reply(200, "User was logged in successfully."));
        }

        [Test]
        public void TestLoginUserWithWrongPassword()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("POST /sessions HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nContent-Type: application/json\r\nContent-Length: 39\r\n\r\n{\"Username\":\"kienboec\", \"Password\":\"d\"}", null);
            db.LoginUser(args.Object);
            args.Verify(e => e.Reply(400, "Error: Couldn't login user!"));
        }

        [Test]
        public void TestCheckIfUserIsAdminTrue()
        {
            DataBase db = new DataBase();
            var mockString = "admin";
            Assert.IsTrue(db.CheckIfUserIsAdmin(mockString));
        }

        [Test]
        public void TestCheckIfUserIsAdminFalse()
        {
            DataBase db = new DataBase();
            var mockString = "somethingelse";
            Assert.IsFalse(db.CheckIfUserIsAdmin(mockString));
        }

        [Test]
        public void TestUserAuthentificationCorrect()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("GET /cards HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nAuthorization: Basic kienboec-mtcgToken\r\n\r\n", null);
            var resultString = db.UserAuthentification(args.Object);
            Assert.That("kienboec", Is.EqualTo(resultString));
        }

        [Test]
        public void TestUserAuthentificationFailure()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("GET /cards HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\n\r\n", null);
            var resultString = db.UserAuthentification(args.Object);
            args.Verify(e => e.Reply(400, "Error: No token given!"));
        }

        [Test]
        public void TestGetUserStatsSuccess()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("GET /stats HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nAuthorization: Basic kienboec-mtcgToken\r\n\r\n", null);

            var userString = db.UserAuthentification(args.Object);

            db.GetUserStats(args.Object, userString);

            args.Verify(e => e.Reply(400, "Unexpected Error occured while trying to get User Stats!"), Times.Never);
        }

        [Test]
        public void TestGetUserStatsFailure()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>("GET /stats HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nAuthorization: Basic someone-mtcgToken\r\n\r\n", null);

            var userString = db.UserAuthentification(args.Object);
            db.GetUserStats(args.Object, userString);

            args.Verify(e => e.Reply(400, "Unexpected Error occured while trying to get User Stats!"));

        }

        [Test]
        public void TestEditUserDataSuccess()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>();
            args.Object.Path = "/users/me";

            var userString = "me";
            db.EditUserData(args.Object, userString);
            args.Verify(e => e.Reply(400, "Something went wrong with the Authorization!"), Times.Never);
        }

        [Test]
        public void TestEditUserDataFailure()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>();
            args.Object.Path = "/users/me";

            var userString = "name";
            db.EditUserData(args.Object, userString);
            args.Verify(e => e.Reply(400, "Something went wrong with the Authorization!"));
        }

        [Test]
        public void TestConfigureDeckLengthFailure()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>();
            args.Object.Payload = "\"[\\\"aa9999a0-734c-49c6-8f4a-651864b14e62\\\", \\\"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\\\", \\\"d60e23cf-2238-4d49-844f-c7589ee5342e\\\", \\\"845f0dc7-37d0-426e-994e-43fc3ac83c08\\\"]\"";
            var userString = "something";

            db.ConfigureDeck(args.Object, userString);
            args.Verify(e => e.Reply(400, "Error: You need to pick 4 cards to configure your deck successfully"));
        }

        [Test]
        public void TestConfigureDeckLengthSuccess()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>();
            args.Object.Payload = "\"[\\\"aa9999a0-734c-49c6-8f4a-651864b14e62\\\", \\\"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\\\", \\\"d60e23cf-2238-4d49-844f-c7589ee5342e\\\", \\\"845f0dc7-37d0-426e-994e-43fc3ac83c08\\\"]\"";
            var userString = "something";

            db.ConfigureDeck(args.Object, userString);
            args.Verify(e => e.Reply(400, "Error: You need to pick 4 cards to configure your deck successfully"), Times.Never);
        }

        [Test]
        public void TestCreateCardsLengthFailure()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>();
            args.Object.Payload = "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, " + "{\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", " + "\"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}]";
            
            db.CreateCards(args.Object);
            args.Verify(e => e.Reply(400, "Error: In order to create a package 5 cards are needed!"));
        }

        [Test]
        public void TestCreateCardsLenghtSuccess()
        {
            DataBase db = new DataBase();
            var args = new Mock<HttpServerEventArgs>();
            args.Object.Payload = "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, " + "{\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", " + "\"Name\":\"WaterSpell\", \"Damage\": 20.0}, " + "{\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"9e8238a4-8a7a-487f-9f7d-a8c97899eb48\", \"Name\":\"Dragon\", \"Damage\": 70.0},]";

            db.CreateCards(args.Object);
            args.Verify(e => e.Reply(400, "Error: In order to create a package 5 cards are needed!"), Times.Never);
        }

        [Test]
        public void TestAquirePackageCoinsEnough()
        {
            DataBase db = new DataBase();
            User user = new User("kienboec", "daniel", 20, "name", "Basic kienboec-mtcgToken");
            var args = new Mock<HttpServerEventArgs>("POST transactions/packages HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nContent-Type: application/json\r\nAuthorization: Basic kienboec-mtcgToken\r\nContent-Length: 0\r\n\r\n", null);
            var userString = "kienboec";

            db.AquirePackage(args.Object, userString);
            args.Verify(e => e.Reply(400, "Error: You need at least 5 coins to aquire a package!"), Times.Never);
        }

        [Test]
        public void TestAquirePackageCoinsNotEnough()
        {
            DataBase db = new DataBase();
            User user = new User("kienboec", "daniel", 1, "name", "Basic kienboec-mtcgToken");
            var args = new Mock<HttpServerEventArgs>("POST transactions/packages HTTP/1.1\r\nHost: localhost:10001\r\nUser-Agent: curl/7.83.1\r\nAccept: */*\r\nContent-Type: application/json\r\nAuthorization: Basic kienboec-mtcgToken\r\nContent-Length: 0\r\n\r\n", null);
            var userString = "kienboec";

            db.AquirePackage(args.Object, userString);
            args.Verify(e => e.Reply(400, "Error: You need at least 5 coins to aquire a package!"));
        }
    }
}
