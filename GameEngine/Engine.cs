using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine
{
    public class EntityAddedEventArgs : EventArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class GameAddedEventArgs : EntityAddedEventArgs
    {
        public string AuthorName { get; set; }
        public string State { get; set; }
        public string Date { get; set; } 
    }

    public class GameChangedEventArgs : EntityAddedEventArgs
    {
        public string GameName { get; set; }
        public string Status { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public int Player1Points { get; set; }
        public int Player2Points { get; set; }
        public string Message { get; set; }
        public string GameWinner { get; set; }
        public string TurnWinner { get; set; }
        public int CurrentTurn { get; set; }
        public int NextTurn { get; set; }
        public string Player { get; set; }
        public int PlayerAction { get; set; }
        public string Time { get; set; }
        public int Player1Action { get; set; }
        public int Player2Action { get; set; }
        public bool IsDraw { get; set; }
        public int PlayersCount { get; set; }
        public bool Player1UsedHint { get; set; }
        public bool Player2UsedHint { get; set; }
    }
    public class EntityRemovedEventArgs : EventArgs
    {
        public Guid Id { get; set; }
    }
    /// <summary>
    /// Singleton класс игры
    /// </summary>
    public class Engine
    {
        public event EventHandler<EntityAddedEventArgs> PlayerAdded;
        public event EventHandler<GameAddedEventArgs> GameAdded;
        public event EventHandler<EntityRemovedEventArgs> PlayerRemoved;
        public event EventHandler<EntityRemovedEventArgs> GameRemoved;
        public event EventHandler<GameChangedEventArgs> GameStarted;
        

        #region Singleton
        private Engine() { }
        private static Engine instance;

        static Engine()
        {
            instance = new Engine();
            instance.Players = new List<Player>();
            instance.Games = new List<Game>();
            instance.Confirure(); 
        }
        public static Engine Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region internal
        public List<Player> Players { get; set; }
        public List<Game> Games { get; set; }

        /// <summary>
        /// Матрица соответствия результатов схватки
        /// </summary>
        internal int[,] gameConfig = new int[5, 5];
        #endregion

        #region Public methods
        /// <summary>
        /// Создать игру
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerName"></param>
        /// <param name="turnDuration"></param>
        /// <param name="victoryLimit"></param>
        public Guid CreateGame(string gameName, string playerName, int turnDuration, int victoryLimit)
        {
            Player player = GetPlayer(playerName);
            player.Points = 0;
            Game game = new Game(gameName, turnDuration, victoryLimit, player);
            game.Players.Add(player);
            player.CurrentGame = game;
            player.HintUsed = false;
            Games.Add(game);

            GameAdded(this, new GameAddedEventArgs() { Id = game.Id, Name = game.Name, AuthorName = playerName, Date = game.CreatedOn.ToShortDateString(), State = "Waiting" });

            return game.Id;
        }
        /// <summary>
        /// Добавить игрока в лист
        /// </summary>
        /// <param name="playerName">Возвращает объект созданного игрока</param>
        public bool AddPlayer(string playerName)
        {
            if(Players.Find(x => x.Name == playerName) == null)
            {
                Player player = new Player(playerName);
                Players.Add(player);
                PlayerAdded(this, new EntityAddedEventArgs() { Id = player.Id, Name = player.Name });
            }
            
            return true;
        }
        /// <summary>
        /// Удалить игрока из списка
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns>Возвращает идентификатор удаленного игрока</returns>
        public bool RemovePlayer(string playerName)
        {
            Player player = GetPlayer(playerName);
            Guid id = player.Id != null ? player.Id : Guid.Empty;
            Players.Remove(player);
            PlayerRemoved(this, new EntityRemovedEventArgs() { Id = id });
            return true;
        }
        /// <summary>
        /// Подключение к игре
        /// </summary>
        /// <param name="gameName">Название игры</param>
        /// <param name="playerName">Имя игрока</param>
        /// <returns></returns>
        public bool ConnectToGame(string gameName, string playerName)
        {
            Game game = GetGameByName(gameName);
            Player player = GetPlayer(playerName);
            if (game != null)
            {
                game.JoinToGame(player);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Подключение к игре
        /// </summary>
        /// <param name="gameName">Название игры</param>
        /// <param name="playerName">Имя игрока</param>
        /// <returns></returns>
        public bool ConnectToGame(Guid id, string playerName)
        {
            Game game = GetGameById(id);
            Player player = GetPlayer(playerName);
            player.Points = 0;
            if (game != null)
            {
                return game.JoinToGame(player);
            }
            return false;
        }
        /// <summary>
        /// Получить список игроков
        /// </summary>
        /// <returns>Список игроков</returns>
        public List<Player> GetPlayers()
        {
            return Players;
        }
        /// <summary>
        /// Получить список игр
        /// </summary>
        /// <returns></returns>
        public List<Game> GetGames()
        {
            return Games;
        }
        /// <summary>
        /// Начать игру
        /// </summary>
        /// <param name="gameName">Название игры</param>
        /// <param name="playerName">Имя игрока</param>
        /// <returns></returns>
        public bool StartGame(string gameName, string playerName)
        {
            Game game = GetGameByName(gameName);
            Player player = GetPlayer(playerName);
            if (game != null)
            {
                game.StartGame(player);
                GameStarted(this, new GameChangedEventArgs() { GameName = gameName, Status = "Game" });
                return true;
            }
            return false;
        }
        /// <summary>
        /// Покинуть игру
        /// </summary>
        /// <param name="gameName">Название игры</param>
        /// <param name="playerName">Имя игрока</param>
        /// <returns></returns>
        public bool LeaveGame(string gameName, string playerName)
        {
            Game game = GetGameByName(gameName);
            Player player = GetPlayer(playerName);
            if (game != null)
            {
                game.LeaveGame(player);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Действие игрока
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool SetAction(string gameName, string playerName, GameAction action)
        {
            Game game = GetGameByName(gameName);
            Player player = GetPlayer(playerName);
            game.SetAction(player, action);
            return true;
        }
        /// <summary>
        /// Получить подсказку
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public int GetHint(string gameName, string playerName)
        {
            Game game = GetGameByName(gameName);
            Player player = GetPlayer(playerName);
            return game.GetHint(player);
        }
        /// <summary>
        /// Удалить игру из списка игр
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool RemoveGame(Game game)
        {
            Engine.Instance.Games.Remove(game);
            GameRemoved(this, new EntityRemovedEventArgs() { Id = game.Id });
            return true;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Конфигурация правил игры
        /// </summary>
        private void Confirure()
        {
            gameConfig[(int)GameAction.Stone, (int)GameAction.Stone] = (int)GameActionResult.Draw;
            gameConfig[(int)GameAction.Stone, (int)GameAction.Paper] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Stone, (int)GameAction.Scissors] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Stone, (int)GameAction.Lizard] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Stone, (int)GameAction.Spock] = (int)GameActionResult.Defeat;

            gameConfig[(int)GameAction.Paper, (int)GameAction.Stone] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Paper, (int)GameAction.Paper] = (int)GameActionResult.Draw;
            gameConfig[(int)GameAction.Paper, (int)GameAction.Scissors] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Paper, (int)GameAction.Lizard] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Paper, (int)GameAction.Spock] = (int)GameActionResult.Victory;

            gameConfig[(int)GameAction.Scissors, (int)GameAction.Stone] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Scissors, (int)GameAction.Paper] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Scissors, (int)GameAction.Scissors] = (int)GameActionResult.Draw;
            gameConfig[(int)GameAction.Scissors, (int)GameAction.Lizard] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Scissors, (int)GameAction.Spock] = (int)GameActionResult.Defeat;

            gameConfig[(int)GameAction.Lizard, (int)GameAction.Stone] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Lizard, (int)GameAction.Paper] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Lizard, (int)GameAction.Scissors] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Lizard, (int)GameAction.Lizard] = (int)GameActionResult.Draw;
            gameConfig[(int)GameAction.Lizard, (int)GameAction.Spock] = (int)GameActionResult.Victory;

            gameConfig[(int)GameAction.Spock, (int)GameAction.Stone] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Spock, (int)GameAction.Paper] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Spock, (int)GameAction.Scissors] = (int)GameActionResult.Victory;
            gameConfig[(int)GameAction.Spock, (int)GameAction.Lizard] = (int)GameActionResult.Defeat;
            gameConfig[(int)GameAction.Spock, (int)GameAction.Spock] = (int)GameActionResult.Draw;
        }
        /// <summary>
        /// Получение игры по имени
        /// </summary>
        /// <param name="gameName">Имя игры</param>
        /// <returns></returns>
        private Game GetGameByName(string gameName)
        {
            return Games.Find(x => x.Name == gameName);
        }
        /// <summary>
        /// Получение игры по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор игры</param>
        /// <returns></returns>
        private Game GetGameById(Guid id)
        {
            return Games.Find(x => x.Id == id);
        }
        /// <summary>
        /// Получение игрока по имени
        /// </summary>
        /// <param name="playerName">Имя игрока</param>
        /// <returns></returns>
        private Player GetPlayer(string playerName)
        {
            return Players.Find(x => x.Name == playerName);
        }
        #endregion
    }

    #region Common classes
    /// <summary>
    /// Класс игрока
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Идентификатор игрока
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Имя игрока
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Количество побед в игре
        /// </summary>
        public int Points { get; set; }
        /// <summary>
        /// Текущая игра
        /// </summary>
        public Game CurrentGame { get; set; }
        /// <summary>
        /// Флаг, показывающий использовал ли игрок подсказку в текущей игре
        /// </summary>
        public bool HintUsed { get; set; }

        public Player(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            Points = 0;
        }
    }
    /// <summary>
    /// Класс игры
    /// </summary>
    public class Game
    {
        public static event EventHandler<GameChangedEventArgs> TurnTimeout;
        public static event EventHandler<GameChangedEventArgs> PlayerConnected;
        public static event EventHandler<GameChangedEventArgs> PlayerAction;
        public static event EventHandler<GameChangedEventArgs> TurnFinished;
        public static event EventHandler<GameChangedEventArgs> GameFinished;
        public static event EventHandler<GameChangedEventArgs> Notification;
        public static event EventHandler<GameChangedEventArgs> StateChanged;
        public static event EventHandler<GameChangedEventArgs> PlayersCountChanged;
        public static event EventHandler<GameChangedEventArgs> PlayerLeaveNotStartedGame;

        /// <summary>
        /// Идентификатор игры
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Дата старта
        /// </summary>
        public DateTime StartedOn { get; set; }
        /// <summary>
        /// Имя игры
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Длительность хода
        /// </summary>
        public int TurnDuration { get; set; }
        /// <summary>
        /// Статус игры
        /// </summary>
        public GameStatus Status { get; set; }
        /// <summary>
        /// Индекс текущего хода
        /// </summary>
        public int CurrentTurn { get; set; }
        /// <summary>
        /// История ходов
        /// </summary>
        public List<Turn> Turns { get; set; }
        /// <summary>
        /// Лимит количества побед
        /// </summary>
        public int VictoryLimit { get; set; }
        /// <summary>
        /// Создатель игры
        /// </summary>
        public Player Creator { get; set; }
        /// <summary>
        /// Победитель
        /// </summary>
        public Player Winner { get; set; }
        /// <summary>
        /// Игроки
        /// </summary>
        public List<Player> Players { get; set; }      

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public Game(string name, int turnDuration, int victoryLimit, Player creator)
        {
            Name = name;
            TurnDuration = turnDuration;
            VictoryLimit = victoryLimit;
            Status = GameStatus.Waiting;
            Turns = new List<Turn>();
            CurrentTurn = 1;
            Players = new List<Player>();
            Creator = creator;
            CreatedOn = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public void StartTimer()
        {
            tokenSource = new CancellationTokenSource();
            var current = CurrentTurn;

            var task = Task.Factory.StartNew(() =>
            {
                tokenSource.Token.ThrowIfCancellationRequested();

                Thread.Sleep(TurnDuration * 1000 + 1500);
                ///1500 это не MagicNumber, а временной лаг, необходимый для показа игрокам ходов противника

                if (!tokenSource.Token.IsCancellationRequested)
                {
                    OnTurnEndTime(current);
                }

            }, tokenSource.Token);
        }
        /// <summary>
        /// Действие при завершении времени хода
        /// </summary>
        private void OnTurnEndTime(int turnNumber)
        {
            if (CurrentTurn != turnNumber)
            {
                return;
            }

            Turn currentTurn = Turns[CurrentTurn - 1];
            int actionCount = currentTurn.GameActions.Count;

            if (actionCount == 0)
            {
                currentTurn.IsDraw = true;
                MakeNotification(string.Format("Turn {0}  - Timeout. Nobody wins", CurrentTurn));

                NextTurn();
            }
            if (actionCount == 1)
            {
                currentTurn.Winner = currentTurn.GameActions[0].Key;
                MakeNotification(string.Format("Turn {0} - Timeout. Player {1} win", Name, CurrentTurn, currentTurn.Winner.Name));
                AddPointsToWinner(currentTurn.Winner);
            }
        }

        /// <summary>
        /// Получить подсказку
        /// </summary>
        /// <returns>Код действия</returns>
        public int GetHint(Player player)
        {
            Turn turn = Turns[CurrentTurn - 1];
            foreach (var item in turn.GameActions)
            {
                if (item.Key != player)
                {
                    turn.HintsUsed.Add(player);
                    player.HintUsed = true;

                    Random rand = new Random();

                    int realAction = (int)item.Value;

                    if (rand.Next(0, 2) == 0)
                    {
                        return realAction;
                    }
                    else
                    {
                        int[] randomActions = new int[] { 0, 1, 2, 3, 4 }; //По количеству возможных действий
                        randomActions = randomActions = randomActions.Where(val => val != realAction).ToArray(); //Исключим из выборки правильный ответ
                        int itemIndex = rand.Next(0, randomActions.Length);
                        return randomActions[itemIndex];
                    }
                }    
            }
            return -1;
        }

        /// <summary>
        /// Подключить игрока к игре
        /// </summary>
        /// <param name="playerName">Игрок</param>
        /// <returns></returns>
        public bool JoinToGame(Player player)
        {
            if (player == Creator)
                return true;
            if (Players.Contains(player))
                return true;
            if (Players.Count != 2 && Status == GameStatus.Waiting)
            {
                Players.Add(player);
                player.CurrentGame = this;
                player.HintUsed = false;
                Status = GameStatus.Waiting;
                MakeNotification(string.Format("Player {0} connect to game {1}", player.Name, Name));
                PlayerConnected(this, new GameChangedEventArgs() { GameName = Name, Player2Name = player.Name });
                PlayersCountChanged(this, new GameChangedEventArgs() { GameName = Name, PlayersCount = Players.Count, Id = Id });
                return true;
            }
            return false;
        }
        /// <summary>
        /// Начать игру
        /// </summary>
        /// <param name="playerName">Игрок</param>
        /// <returns></returns>
        public bool StartGame(Player player)
        {
            if (Players.Count < 1)
            {
                return false;
            }
            if (Creator == player)
            {
                Status = GameStatus.Game;
                StartedOn = DateTime.Now;
                Turns.Add(new Turn());
                StartTimer();
                MakeNotification(string.Format("Game {0} started", Name));
                StateChanged(this, new GameChangedEventArgs() { GameName = Name, Status = "Game", Id = Id });
                return true;
            }
            return false;
        }
        /// <summary>
        /// Обработка действия игрока
        /// </summary>
        /// <param name="player">Игрок</param>
        /// <param name="action">Действие</param>
        public void SetAction(Player player, GameAction action)
        {
            if (Status != GameStatus.Game)
                return;
            

            var gameActions = Turns[CurrentTurn - 1].GameActions;
            if (gameActions.Count < 2)
            {
                gameActions.Add(new KeyValuePair<Player, GameAction>(player, action));
            }

            if (gameActions.Count == 2)
            {
                Player player1 = Players[0];
                Player player2 = Players[1];
                Player winner = null;

                GameAction player1Action = gameActions.Find(x => x.Key.Name == Players[0].Name).Value;
                GameAction player2Action = gameActions.Find(x => x.Key.Name == Players[1].Name).Value;
                GameActionResult result = (GameActionResult)Engine.Instance.gameConfig[(int)player1Action, (int)player2Action];

                switch (result)
                {
                    case GameActionResult.Draw:
                        Turns[CurrentTurn - 1].IsDraw = true;
                        MakeNotification(string.Format("Turn {0} - nobody win!", CurrentTurn));
                        SendPlayerActionsInformation();
                        NextTurn();
                        break;
                    case GameActionResult.Victory:
                        winner = player1;
                        break;
                    case GameActionResult.Defeat:
                        winner = player2;
                        break;
                }
                if (winner != null)
                {
                    AddPointsToWinner(winner);
                }
            }
        }
        /// <summary>
        /// Инфомирование о действия игроков
        /// </summary>
        private void SendPlayerActionsInformation()
        {
            Turn currentTurn = Turns[CurrentTurn - 1];

            Player player1 = Players[0];
            Player player2 = Players[1];

            var player1Action = currentTurn.GameActions.Find(x => x.Key == player1);
            var player2Action = currentTurn.GameActions.Find(x => x.Key == player2);

            TurnFinished(this, new GameChangedEventArgs()
            {
                GameName = Name,
                Player1Points = player1.Points,
                Player2Points = player2.Points,
                CurrentTurn = CurrentTurn,
                NextTurn = CurrentTurn + 1,
                TurnWinner = currentTurn.Winner != null ? currentTurn.Winner.Name : "nobody",
                Time = DateTime.Now.ToShortTimeString(),
                Player1Action = player1Action.Key == null ? -1 : (int)player1Action.Value,
                Player2Action = player2Action.Key == null ? -1 : (int)player2Action.Value,
                Player1Name = player1.Name,
                Player2Name = player2.Name,
                Player1UsedHint = currentTurn.HintsUsed.Contains(player1),
                Player2UsedHint = currentTurn.HintsUsed.Contains(player2),
            });
        }
        /// <summary>
        /// Выйти из игры
        /// </summary>
        /// <param name="player">Игрок</param>
        public void LeaveGame(Player player)
        {
            if(Players.Count == 2 && Status == GameStatus.Waiting)
            {
                PlayerLeaveNotStartedGame(this, new GameChangedEventArgs() { GameName = Name, Player = player.Name });
            }
            if (Players.Count == 2 && Status == GameStatus.Game)
            {
                Winner = Players.Find(x => x != player);
                Status = GameStatus.Finished;
                StateChanged(this, new GameChangedEventArgs() { GameName = Name, Status = "Finished", Id = Id });

                MakeNotification(string.Format("Player {0} exit game. Player {1} win!", player.Name, Winner.Name));
            }
            RemoveFromPlayers(player);
        }
        /// <summary>
        /// Удалить игрока из списка
        /// </summary>
        /// <param name="player">Игрок</param>
        /// <returns></returns>
        public bool RemoveFromPlayers(Player player)
        {
            Players.Remove(player);
            player.CurrentGame = null;
            player.HintUsed = false;
            MakeNotification(string.Format("Player {0} removed from player list", player.Name));
            PlayersCountChanged(this, new GameChangedEventArgs() { GameName = Name, PlayersCount = Players.Count, Id = Id });
            if (Players.Count == 0)
            {
                Debug.WriteLine("All player exit game {0}. Game will deleted", Name);
                Engine.Instance.RemoveGame(this);
            }

            return true;
        }
        /// <summary>
        /// Начислить очки игроку
        /// </summary>
        /// <param name="player">Игрок</param>
        private void AddPointsToWinner(Player player)
        {
            player.Points++;
            MakeNotification(string.Format("{0} wins round", player.Name));

            Turn currentTurn = Turns[CurrentTurn - 1];
            currentTurn.Winner = player;

            SendPlayerActionsInformation();

            if (player.Points == VictoryLimit)
            {
                Winner = player;
                MakeNotification(string.Format("{0} win the game!", Winner.Name));
                GameFinished(this, new GameChangedEventArgs()
                {
                    GameName = Name,
                    GameWinner = Winner.Name,
                    Time = DateTime.Now.ToShortTimeString()
                });
                FinishGame();
            }
            NextTurn();
        }
        /// <summary>
        /// Завершить игру
        /// </summary>
        private void FinishGame()
        {
            Status = GameStatus.Finished;
            StateChanged(this, new GameChangedEventArgs() { GameName = Name, Status = "Finished", Id = Id });
        }
        /// <summary>
        /// Перейти к следующему ходу
        /// </summary>
        private void NextTurn()
        {
            tokenSource.Cancel();
            //tokenSource.Token.ThrowIfCancellationRequested();

            if (Status == GameStatus.Game)
            {
                Turns.Add(new Turn());
                CurrentTurn++;
                MakeNotification(string.Format("Turn {0} started", CurrentTurn));
                StartTimer();
            }
        }
        private void MakeNotification(string message)
        {
            Notification(this, new GameChangedEventArgs() { GameName = Name, Message = message });
            Debug.WriteLine(message);
        }
        /// <summary>
        /// Получить текущие очки игрока
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        private int GetPlayerPoints(int playerIndex)
        {
            try
            {
                return Players[playerIndex].Points;
            }
            catch (Exception)
            {
                return 0;
            }
            
        }
    }
    /// <summary>
    /// Класс, реализующий хранение истории ходов
    /// </summary>
    public class Turn
    {
        public DateTime Date { get; set; }
        public List<KeyValuePair<Player, GameAction>> GameActions;
        public Player Winner { get; set; }
        public List<Player> HintsUsed { get; set; }
        public bool IsDraw { get; set; }

        public Turn()
        {
            GameActions = new List<KeyValuePair<Player, GameAction>>();
            HintsUsed = new List<Player>();
        }
    }
    #endregion

    #region Enums
    /// <summary>
    /// Статус игры
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// Ожидание
        /// </summary>
        Waiting,
        /// <summary>
        /// Готовность к игре
        /// </summary>
        Ready,
        /// <summary>
        /// Идет игра
        /// </summary>
        Game,
        /// <summary>
        /// Игра завершена
        /// </summary>
        Finished
    }
    /// <summary>
    /// Игровое действие
    /// </summary>
    public enum GameAction
    {
        /// <summary>
        /// Камень
        /// </summary>
        Stone,
        /// <summary>
        /// Бумага
        /// </summary>
        Paper,
        /// <summary>
        /// Ножницы
        /// </summary>
        Scissors,
        /// <summary>
        /// Ящерица
        /// </summary>
        Lizard,
        /// <summary>
        /// Спок
        /// </summary>
        Spock
    }
    /// <summary>
    /// Результат игрового действия
    /// </summary>
    public enum GameActionResult
    {
        /// <summary>
        /// Ничья
        /// </summary>
        Draw,
        /// <summary>
        /// Победа
        /// </summary>
        Victory,
        /// <summary>
        /// Поражение
        /// </summary>
        Defeat
    }
    #endregion

}
