using GameEngine;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebClient.WebAPI
{
    public enum GameMethod
    {
        AddPlayer,
        ConnectToGame,
        CreateGame,
        LeaveGame,
        RemovePlayer,
        SetAction,
        StartGame,
        GetHint
    }
    public class GameRequest
    {
        public GameMethod Method { get; set; }
        public string PlayerName { get; set; }
        public string GameName { get; set; }
        public int TurnDuration { get; set; }
        public int VictoryLimit { get; set; }
        public GameAction Action { get; set; }
    }
    [RoutePrefix("api/Game")]
    public class GameController : ApiController
    {
        public object GameRequest(GameRequest request)
        {
            switch (request.Method)
            {
                case GameMethod.AddPlayer:
                    return AddPlayer(request.PlayerName);
                    break;
                case GameMethod.ConnectToGame:
                    return ConnectToGame(request.GameName, request.PlayerName);
                    break;
                case GameMethod.CreateGame:
                    return CreateGame(request.GameName, request.PlayerName, request.TurnDuration, request.VictoryLimit) != Guid.Empty;
                    break;
                case GameMethod.LeaveGame:
                    return LeaveGame(request.GameName, request.PlayerName);
                    break;
                case GameMethod.RemovePlayer:
                    return RemovePlayer(request.PlayerName);
                    break;
                case GameMethod.SetAction:
                    return SetAction(request.GameName, request.PlayerName, request.Action);
                    break;
                case GameMethod.StartGame:
                    return StartGame(request.GameName, request.PlayerName);
                    break;
                case GameMethod.GetHint:
                    return GetHint(request.GameName, request.PlayerName);
                    break;
                default:
                    return false;
                    break;
            }
        }

        private int GetHint(string gameName, string playerName)
        {
            return GameEngine.Engine.Instance.GetHint(gameName, playerName);
        }
        private bool AddPlayer(string playerName)
        {
            return GameEngine.Engine.Instance.AddPlayer(playerName);
        }
        private bool RemovePlayer(string playerName)
        {
            return GameEngine.Engine.Instance.RemovePlayer(playerName);
        }

        private bool ConnectToGame(string gameName, string name)
        {
            return GameEngine.Engine.Instance.ConnectToGame(gameName, name);
        }

        private Guid CreateGame(string gameName, string playerName, int turnDuration, int victoryLimit)
        {
            return GameEngine.Engine.Instance.CreateGame(gameName, playerName, turnDuration, victoryLimit);
        }

        private bool LeaveGame(string gameName, string playerName)
        {
            return GameEngine.Engine.Instance.LeaveGame(gameName, playerName);
        }

        private bool SetAction(string gameName, string playerName, GameAction action)
        {
            return GameEngine.Engine.Instance.SetAction(gameName, playerName, action);
        }

        private bool StartGame(string gameName, string playerName)
        {
            return GameEngine.Engine.Instance.StartGame(gameName, playerName);
        }


    }
}
