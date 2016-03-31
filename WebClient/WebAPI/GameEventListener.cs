using GameEngine;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace WebClient.WebAPI
{
    /// <summary>
    /// Класс, следящий за событиями в игре, и уведомляющий пользователей о событиях
    /// </summary>
    public class GameEventListener
    {
        #region Singleton
        private GameEventListener() { }
        private static GameEventListener instance;

        static GameEventListener()
        {
            instance = new GameEventListener();
            instance.Configure();
        }
        public static GameEventListener Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion
        /// <summary>
        /// Инициализация
        /// </summary>
        public void Initialize()
        {
            Debug.WriteLine("GameEventListener initialized");
        }
        /// <summary>
        /// Подписка на события игры
        /// </summary>
        private void Configure()
        {
            GameEngine.Engine.Instance.GameAdded += OnGameAdded;
            GameEngine.Engine.Instance.PlayerAdded += OnPlayerAdded;
            GameEngine.Engine.Instance.PlayerRemoved += OnPlayerRemoved;
            GameEngine.Engine.Instance.GameStarted += OnGameStarted;
            GameEngine.Engine.Instance.GameRemoved += OnGameRemoved;
            Game.TurnTimeout += OnTurnTimeout;
            Game.PlayerConnected += OnPlayerConnected;
            Game.Notification += OnNotification;
            Game.TurnFinished += OnTurnFinished;
            Game.TurnFinished += LogTurn;
            Game.PlayerAction += OnPlayerAction;
            Game.GameFinished += OnGameFinished;
            Game.PlayersCountChanged += OnPlayersCountChanged;
            Game.StateChanged += OnStateChanged;
            Game.PlayerLeaveNotStartedGame += OnPlayerLeaveNotStartedGame;
        }
        /// <summary>
        /// Логирование ходов, формирование разметки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void LogTurn(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).turnLog(
                new
                {
                    CurrentTurn = args.CurrentTurn,
                    TurnWinner = args.TurnWinner,
                    Player1Name = args.Player1Name,
                    Player1Action = GetActionByCode(args.Player1Action),
                    Player1UsedHint = args.Player1UsedHint,
                    Player2Name = args.Player2Name,
                    Player2Action = GetActionByCode(args.Player2Action),
                    Player2UsedHint = args.Player2UsedHint
                }
            );
        }
        private string GetActionByCode(int code)
        {
            switch (code)
            {
                case 0:
                    return "Stone";
                case 1:
                    return "Paper";
                case 2:
                    return "Scissors";
                case 3:
                    return "Lizard";
                case 4:
                    return "Spock";
                default:
                    return "";
                    break;
            }
        }
        private void OnGameRemoved(object sender, EntityRemovedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            context.Clients.All.removeGame(new
            {
                id = args.Id
            });
        }
        /// <summary>
        /// Обработкчик события, когда игрок покинул не начатую игру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPlayerLeaveNotStartedGame(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onPlayerLeaveNotStartedGame(new { PlayerName = args.Player });
        }
        /// <summary>
        /// Обработчик события изменения состояния игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnStateChanged(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onStateChanged(new { State = args.Status, GameId = args.Id });

            var context2 = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            context2.Clients.All.changeGameState(new
            {
                GameId = args.Id,
                State = args.Status
            });
        }
        /// <summary>
        /// Оповещение таблицы игр об изменении количества игроков в игре.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPlayersCountChanged(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onPlayersCountChanged(new { PlayersCount = args.PlayersCount, GameId = args.Id });

            var context2 = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            context2.Clients.All.updateGamePlayers(new
            {
                GameId = args.Id,
                PlayersCount = args.PlayersCount + "/2"
            });
        }

        /// <summary>
        /// Обработчик события поступления уведомления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNotification(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onNotification(new { Message = args.Message, Time = DateTime.Now.ToShortTimeString() } );
        }
        /// <summary>
        /// Обработчик события завершения хода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTurnFinished(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onTurnFinished(
                new {
                    turnWinner = args.TurnWinner,
                    player1Points = args.Player1Points,
                    player2Points = args.Player2Points,
                    currentTurn = args.CurrentTurn,
                    nextTurn = args.NextTurn,
                    time = args.Time,
                    player1Action = args.Player1Action,
                    player2Action = args.Player2Action,
                    player1Name = args.Player1Name,
                    player2Name = args.Player2Name,
                    isDraw = args.IsDraw
                }    
            );
        }
        /// <summary>
        /// Обработчик события завершения игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGameFinished(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onGameFinished(
                new
                {
                    gameWinner = args.GameWinner,
                    time = args.Time
                }
            );
        }
        /// <summary>
        /// Обработчик события действия игрока
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPlayerAction(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onPlayerAction(args.Status);
        }
        /// <summary>
        /// Действие при добавлении игрока в список
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPlayerAdded(object sender, EntityAddedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayerHub>();
            context.Clients.All.addPlayer(new { id = args.Id, name = args.Name });
        }
        /// <summary>
        /// Действие при удалении игрока из списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPlayerRemoved(object sender, EntityRemovedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayerHub>();
            context.Clients.All.removePlayer(new { id = args.Id });
        }
        /// <summary>
        /// Действие при добавлении игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGameAdded(object sender, GameAddedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            context.Clients.All.addGame(new
            {
                id = args.Id,
                name = args.Name,
                author = args.Name,
                date = args.Date,
                state = args.State
            });
        }
        /// <summary>
        /// Действие при старте игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGameStarted(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onGameStarted();
        }
        /// <summary>
        /// Действие при таймауте хода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTurnTimeout(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).onTurnTimeout(new
            {
                turn = args.NextTurn
            });
        }
        /// <summary>
        /// Действие при подключении игрока к игре
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPlayerConnected(object sender, GameChangedEventArgs args)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
            context.Clients.Group(args.GameName).addPlayer2(new { Player2Name = args.Player2Name });
            //TODO сообщить в лобби
        }


    }
}