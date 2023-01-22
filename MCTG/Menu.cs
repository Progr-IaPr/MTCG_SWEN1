using System;
using System.Collections.Generic;
using MTCG.User;

namespace MTCG
{
	public class Menu
	{
        public void Menu()
        {
            System.Console.WriteLine("Welcome to the Monster Trading Card Game. Are you already a user or do you need to register first?\n");
            System.Console.WriteLine("[1]I'm new here\n [2]I'm already a user here\n");
            char userInput = Convert.ToChar(Console.ReadLine());

            while (true)
            {
                if (userInput == '1')
                {
                    Console.WriteLine("Alright newbie, just enter your username and password and we're good to go");
                    //todo register user to db
                }
                else if (userInput == '2')
                {
                    Console.WriteLine("Welcome back! Once you enter your username and password, we know who you are :)");
                    //todo user login
                }
                else
                {
                    Console.WriteLine("Sorry, your input doesn't make any sense. Try again...");
                }
            }
        }
    
    }
}
