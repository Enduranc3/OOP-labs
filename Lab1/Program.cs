namespace Lab1
{
    public class Game
    {
        private static int _idCounter;
        private readonly int _gameId;
        public string Player1 { get; }
        public string Player2 { get; }
        public string Winner { get; }
        public int RatingChange { get; }
        public Game(string player1, string player2, string winner, int ratingChange)
        {
            _gameId = ++_idCounter;
            Player1 = player1;
            Player2 = player2;
            Winner = winner;
            RatingChange = ratingChange;
        }
        public int GameId => _gameId;
    }
    public class GameAccount
    {
        public string UserName { get; }
        public int CurrentRating { get; private set; }
        private uint GamesCount { get; set; }
        private List<Game> GameHistory { get; }

        public GameAccount(string userName, int currentRating)
        {
            if (currentRating < 1)
            {
                throw new ArgumentException("Initial rating must be at least 1.");
            }
            UserName = userName;
            CurrentRating = currentRating;
            GamesCount = 0;
            GameHistory = new List<Game>();
        }

        public void WinGame(GameAccount opponent, int rating)
        {
            if (rating < 0)
            {
                throw new ArgumentException("Rating cannot be negative.");
            }
            CurrentRating += rating;
            if (opponent.CurrentRating - rating < 1)
            {
                opponent.CurrentRating = 1;
            }
            else
            {
                opponent.CurrentRating -= rating;
            }
            GamesCount++;
            opponent.GamesCount++;
            var game = new Game(UserName, opponent.UserName, UserName, rating);
            GameHistory.Add(game);
            opponent.GameHistory.Add(game);
        }

        public void LoseGame(GameAccount opponent, int rating)
        {
            if (rating < 0)
            {
                throw new ArgumentException("Rating cannot be negative.");
            }
            opponent.CurrentRating += rating;
            if (CurrentRating - rating < 1)
            {
                CurrentRating = 1;
            }
            else
            {
                CurrentRating -= rating;
            }
            GamesCount++;
            opponent.GamesCount++;
            var game = new Game(UserName, opponent.UserName, opponent.UserName, rating);
            GameHistory.Add(game);
            opponent.GameHistory.Add(game);
        }
        public void PrintStats()
        {
            Console.WriteLine($"{"Game ID",-10}| {"1st player",-15}| {"2nd player",-15}| {"Winner",-10}| {"Rating change",-15}|");
            foreach (var game in GameHistory)
            {
                Console.WriteLine(
                    $"{game.GameId,-10}| {game.Player1,-15}| {game.Player2,-15}| {game.Winner,-10}| {(game.Winner == UserName ? "+" : "-")}{game.RatingChange,-14}|");
            }
            Console.WriteLine($"\nGames played: {GamesCount}");
        }
    }
    class Program
    {
        private static void Main()
        {
            var player1 = new GameAccount("Player1", new Random().Next(1, 1000));
            var player2 = new GameAccount("Player2", new Random().Next(1, 1000));
            var player3 = new GameAccount("Player3", new Random().Next(1, 1000));

            Console.WriteLine($"Current Rating of {player1.UserName}: {player1.CurrentRating}");
            Console.WriteLine($"Current Rating of {player2.UserName}: {player2.CurrentRating}");
            Console.WriteLine($"Current Rating of {player3.UserName}: {player3.CurrentRating}\n");

            player1.WinGame(player2, new Random().Next(1, 30));
            player2.WinGame(player1, new Random().Next(1, 30));
            player1.LoseGame(player2, new Random().Next(1, 30));
            player3.WinGame(player1, new Random().Next(1, 30));
            player3.LoseGame(player2, new Random().Next(1, 30));

            Console.WriteLine("Player1 Stats:");
            player1.PrintStats();

            Console.WriteLine("\nPlayer2 Stats:");
            player2.PrintStats();

            Console.WriteLine("\nPlayer3 Stats:");
            player3.PrintStats();

            Console.WriteLine($"\nCurrent Rating of {player1.UserName}: {player1.CurrentRating}");
            Console.WriteLine($"Current Rating of {player2.UserName}: {player2.CurrentRating}");
            Console.WriteLine($"Current Rating of {player3.UserName}: {player3.CurrentRating}");
        }
    }
}