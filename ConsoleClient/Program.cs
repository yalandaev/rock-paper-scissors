using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = GameEngine.Engine.Instance;
            game.AddPlayer("Player1");
            game.AddPlayer("Player2");

            game.CreateGame("Game1", "Player1", 5, 5);

            game.ConnectToGame("Game1", "Player1");
            game.ConnectToGame("Game1", "Player2");

            game.StartGame("Game1", "Player1");

            //1
            game.SetAction("Game1", "Player1", GameEngine.GameAction.Stone);
            game.SetAction("Game1", "Player2", GameEngine.GameAction.Paper);

            Thread.Sleep(3000);

            //2
            game.SetAction("Game1", "Player1", GameEngine.GameAction.Stone);

            Thread.Sleep(6000);

            //game.SetAction("Game1", "Player2", GameEngine.GameAction.Paper);

            //Thread.Sleep(500);

            //3
            game.SetAction("Game1", "Player1", GameEngine.GameAction.Stone);
            game.SetAction("Game1", "Player2", GameEngine.GameAction.Paper);

            Thread.Sleep(3000);

            //game.LeaveGame("Game1", "Player2");


            //4
            game.SetAction("Game1", "Player1", GameEngine.GameAction.Stone);
            game.SetAction("Game1", "Player2", GameEngine.GameAction.Paper);

            Thread.Sleep(3000);


            //5
            game.SetAction("Game1", "Player1", GameEngine.GameAction.Stone);
            game.SetAction("Game1", "Player2", GameEngine.GameAction.Paper);

            Thread.Sleep(3000);

            //6
            game.SetAction("Game1", "Player1", GameEngine.GameAction.Stone);
            game.SetAction("Game1", "Player2", GameEngine.GameAction.Paper);

            game.LeaveGame("Game1", "Player1");
            game.LeaveGame("Game1", "Player2");

            Console.ReadKey();
        }
    }
}
