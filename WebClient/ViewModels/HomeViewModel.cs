using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebClient.ViewModels
{
    public class HomeViewModel
    {
        public List<Player> Players { get; set; }
        public List<Game> Games { get; set; }

        public HomeViewModel()
        {
            Players = GameEngine.Engine.Instance.GetPlayers();
            Games = GameEngine.Engine.Instance.GetGames();
        }
    }
}