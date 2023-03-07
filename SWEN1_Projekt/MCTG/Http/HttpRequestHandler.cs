using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using MTCG.DB;
using MTCG.Models;

namespace MTCG.Http
{
    public class HttpRequestHandler
    {
        public static void _Svr_Incoming(object evt)
        {
            HttpServerEventArgs e = (HttpServerEventArgs)evt;
            DataBase db = new DataBase();
            Authentification auth = new Authentification();
            bool needAuth = true;

            if((e.Path == "/users" && e.Method == "POST") || (e.Path == "/sessions" && e.Method == "POST"))
            {
                needAuth = false;
            }

            if(needAuth)
            {
                auth.UserString = db.UserAuthentification(e);
            }

            if (e.Path == "/users" && e.Method == "POST")
            {
                db.CreateUser(e);
            }
            else if(e.Path == "/sessions" && e.Method == "POST")
            {
                db.LoginUser(e);
            }
            else if (!string.IsNullOrEmpty(auth.UserString))
            {
                
                if (e.Path == "/packages" && e.Method == "POST")
                {
                    bool isAdmin = db.CheckIfUserIsAdmin(auth.UserString);
                    if (!isAdmin)
                    {
                        e.Reply(400, "Error: Only the admin can create packages!");
                    }
                    else
                    {
                        db.CreateCards(e);
                    }
                }
                else if (e.Path == "/transactions/packages" && e.Method == "POST")
                {
                    db.AquirePackage(e, auth.UserString);  
                }
                else if (e.Path == "/cards" && e.Method == "GET")
                {
                    db.GetCards(e, auth.UserString);  
                }
                else if (e.Path == "/deck" && e.Method == "GET")
                {
                    db.GetDeck(e, auth.UserString);
                }
                else if (e.Path == "/deck" && e.Method == "PUT")
                {
                    db.ConfigureDeck(e, auth.UserString); 
                }
                else if(e.Path.StartsWith("/users/") && e.Method == "GET")
                {
                    db.GetUserData(e, auth.UserString);
                }
                else if(e.Path.StartsWith("/users/") && e.Method == "PUT")
                {
                    db.EditUserData(e, auth.UserString);
                }
                else if(e.Path == "/stats" && e.Method == "GET")
                {
                    db.GetUserStats(e, auth.UserString);
                }
                else if(e.Path == "/score" && e.Method == "GET")
                {
                    db.GetScoreboard(e, auth.UserString);
                }
            }    
        }
    }
}
