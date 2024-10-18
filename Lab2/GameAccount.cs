namespace Lab2
{
    public abstract class GameAccount
    {
        protected const int MinRating = 1;
        private static readonly Dictionary<Guid, GameAccount> Accounts = new();
        public Guid UserId { get; }
        public string UserName { get; }
        public int CurrentRating { get; protected set; }
        protected List<Game> GameHistory { get; }
        protected Dictionary<Guid, int> RatingHistory { get; }

        protected GameAccount(string userName, int currentRating)
        {
            UserId = Guid.NewGuid();
            UserName = userName;
            CurrentRating = currentRating;
            GameHistory = new List<Game>();
            RatingHistory = new Dictionary<Guid, int>();
            Accounts[UserId] = this;
        }
        public virtual void WinGame(Game game)
        {
            if (game.WinnerId != UserId)
            {
                throw new ArgumentException("Invalid game.");
            }
            CurrentRating += game.GetSignedRatingChange(this);
            RatingHistory[game.GameId] = game.GetSignedRatingChange(this);
            GameHistory.Add(game);
        }
        public virtual void LoseGame(Game game)
        {
            if (game.WinnerId == UserId)
            {
                throw new ArgumentException("Invalid game.");
            }
            var predictedRating = Math.Max(MinRating, CurrentRating + game.GetSignedRatingChange(this));
            if (predictedRating == MinRating)
            {
                RatingHistory[game.GameId] = MinRating - CurrentRating;
                CurrentRating = MinRating;
            }
            else
            {
                RatingHistory[game.GameId] = game.GetSignedRatingChange(this);
                CurrentRating = predictedRating;
            }
            GameHistory.Add(game);
        }
        public virtual void PrintStats()
        {
            Console.WriteLine(
                $"{"Game ID",-40}| {"1st player",-15}| {"2nd player",-15}| {"Rating change",-16}| {"Game type",-16}|");

            foreach (var game in GameHistory)
            {
                var player1Color = game.WinnerId == game.Player1Id ? ConsoleColor.Green : ConsoleColor.Red;
                var player2Color = game.WinnerId == game.Player2Id ? ConsoleColor.Green : ConsoleColor.Red;

                var ratingChange = RatingHistory[game.GameId];

                var ratingChangeColor = ratingChange > 0 ? ConsoleColor.Green :
                    ratingChange < 0 ? ConsoleColor.Red : ConsoleColor.Yellow;

                var gameTypeColor = game.GameType switch
                {
                    GameType.Standard => ConsoleColor.Magenta,
                    GameType.Training => ConsoleColor.Cyan,
                    GameType.SingleRating => ConsoleColor.DarkBlue,
                    _ => ConsoleColor.White
                };

                Console.Write($"{game.GameId,-40}| ");

                Console.ForegroundColor = player1Color;
                Console.Write($"{GetAccountById(game.Player1Id).UserName,-15}");
                Console.ResetColor();

                Console.Write("| ");

                Console.ForegroundColor = player2Color;
                Console.Write($"{GetAccountById(game.Player2Id).UserName,-15}");
                Console.ResetColor();

                Console.Write("| ");

                Console.ForegroundColor = ratingChangeColor;
                switch (ratingChange)
                {
                    case > 0:
                        Console.Write($"+{ratingChange,-15}");
                        break;
                    default:
                        Console.Write($"{ratingChange,-16}");
                        break;
                }
                Console.ResetColor();

                Console.Write("| ");

                Console.ForegroundColor = gameTypeColor;
                Console.Write($"{game.GameType,-16}");
                Console.ResetColor();

                Console.WriteLine("|");
            }
            Console.Write($"{"| ",+76}");
            Console.Write($"Games played: {GameHistory.Count} ");
            Console.WriteLine("|");
        }
        public static GameAccount GetAccountById(Guid userId)
        {
            if (Accounts.TryGetValue(userId, out var account))
            {
                return account;
            }
            throw new ArgumentException("Invalid user ID.");
        }
    }

    public class StandardGameAccount(string userName, int currentRating) : GameAccount(userName, currentRating);

    public class HalfLossGameAccount(string userName, int currentRating) : GameAccount(userName, currentRating)
    {
        public override void LoseGame(Game game)
        {
            if (game.WinnerId == UserId)
            {
                throw new ArgumentException("Invalid game.");
            }
            var predictedRating = Math.Max(MinRating, CurrentRating + game.GetSignedRatingChange(this) / 2);
            if (predictedRating == MinRating)
            {
                RatingHistory[game.GameId] = MinRating - CurrentRating;
                CurrentRating = MinRating;
            }
            else
            {
                RatingHistory[game.GameId] = game.GetSignedRatingChange(this) / 2;
                CurrentRating = predictedRating;
            }
            GameHistory.Add(game);
        }
    }

    public class StreakBonusGameAccount(string userName, int currentRating) : GameAccount(userName, currentRating)
    {
        private int WinStreak { get; set; }
        private Dictionary<Guid, int> WinStreakHistory { get; } = new();

        public override void WinGame(Game game)
        {
            if (game.WinnerId != UserId)
            {
                throw new ArgumentException("Invalid game.");
            }

            CurrentRating += game.GetSignedRatingChange(this) + WinStreak;
            RatingHistory[game.GameId] = game.GetSignedRatingChange(this) + WinStreak;

            GameHistory.Add(game);
            UpdateWinStreak(game);
        }
        public override void LoseGame(Game game)
        {
            base.LoseGame(game);
            UpdateWinStreak(game);
        }
        private void UpdateWinStreak(Game game)
        {
            if (game.GameType is GameType.Standard or GameType.SingleRating && game.WinnerId == UserId)
            {
                WinStreak++;
            }
            else if (game.GameType != GameType.Training)
            {
                WinStreak = 0;
            }
            WinStreakHistory[game.GameId] = WinStreak;
        }

        public override void PrintStats()
        {
            Console.WriteLine(
                $"{"Game ID",-40}| {"1st player",-15}| {"2nd player",-15}| {"Rating change",-16}| {"Game type",-16}| {"Streak",-7}|");

            foreach (var game in GameHistory)
            {
                var player1Color = game.WinnerId == game.Player1Id ? ConsoleColor.Green : ConsoleColor.Red;
                var player2Color = game.WinnerId == game.Player2Id ? ConsoleColor.Green : ConsoleColor.Red;

                var ratingChange = RatingHistory[game.GameId];

                var ratingChangeColor = ratingChange > 0 ? ConsoleColor.Green :
                    ratingChange < 0 ? ConsoleColor.Red : ConsoleColor.Yellow;

                var gameTypeColor = game.GameType switch
                {
                    GameType.Standard => ConsoleColor.Magenta,
                    GameType.Training => ConsoleColor.Cyan,
                    GameType.SingleRating => ConsoleColor.DarkBlue,
                    _ => ConsoleColor.White
                };

                Console.Write($"{game.GameId,-40}| ");

                Console.ForegroundColor = player1Color;
                Console.Write($"{GetAccountById(game.Player1Id).UserName,-15}");
                Console.ResetColor();

                Console.Write("| ");

                Console.ForegroundColor = player2Color;
                Console.Write($"{GetAccountById(game.Player2Id).UserName,-15}");
                Console.ResetColor();

                Console.Write("| ");

                Console.ForegroundColor = ratingChangeColor;
                switch (ratingChange)
                {
                    case > 0:
                        Console.Write($"+{ratingChange,-15}");
                        break;
                    default:
                        Console.Write($"{ratingChange,-16}");
                        break;
                }
                Console.ResetColor();

                Console.Write("| ");

                Console.ForegroundColor = gameTypeColor;
                Console.Write($"{game.GameType,-16}");
                Console.ResetColor();


                if (WinStreakHistory.TryGetValue(game.GameId, out var winStreak))
                {
                    Console.Write($"| {winStreak,-7}");
                }

                Console.WriteLine("|");
            }
            Console.Write($"{"| ",+76}");
            Console.Write($"Games played: {GameHistory.Count} ");
            Console.WriteLine("|");
        }
    }
}