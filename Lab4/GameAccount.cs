namespace Lab4;

public abstract class GameAccount(string userName, int currentRating)
{
    protected const int MinRating = 1;

    public Guid UserId { get; } = Guid.NewGuid();
    public string UserName { get; } = userName;
    public int CurrentRating { get; protected set; } = currentRating;

    public List<Game> GameHistory(IGameRepository gameRepository) => gameRepository.ReadAll()
        .Where(game => game.Player1Id == UserId || game.Player2Id == UserId).ToList();

    protected Dictionary<Guid, int> RatingHistory(IGameRepository gameRepository)
    {
        var dbContext = gameRepository.GetDbContext();
        if (dbContext.RatingHistories.TryGetValue(UserId, out var value))
        {
            return value;
        }
        value = [];
        dbContext.RatingHistories[UserId] = value;
        return value;
    }

    public virtual void WinGame(IGameRepository gameRepository, Game game)
    {
        if (game.WinnerId != UserId)
        {
            throw new ArgumentException("Invalid game.");
        }

        CurrentRating += game.GetSignedRatingChange(this);
        RatingHistory(gameRepository)[game.GameId] = game.GetSignedRatingChange(this);
        GameHistory(gameRepository).Add(game);
    }

    public virtual void LoseGame(IGameRepository gameRepository, Game game)
    {
        if (game.WinnerId == UserId)
        {
            throw new ArgumentException("Invalid game.");
        }

        var predictedRating = Math.Max(MinRating, CurrentRating + game.GetSignedRatingChange(this));
        if (predictedRating == MinRating)
        {
            RatingHistory(gameRepository)[game.GameId] = MinRating - CurrentRating;
            CurrentRating = MinRating;
        }
        else
        {
            RatingHistory(gameRepository)[game.GameId] = game.GetSignedRatingChange(this);
            CurrentRating = predictedRating;
        }

        GameHistory(gameRepository).Add(game);
    }

    protected static string GetSafePlayerName(PlayerService playerService, IGameAccountRepository repository,
        Guid playerId)
    {
        return playerService.ReadAccountById(playerId)?.UserName ?? "Unknown Player";
    }

    public virtual void PrintStats(PlayerService playerService, IGameAccountRepository repository,
        IGameRepository gameRepository)
    {
        if (GameHistory(gameRepository).Count == 0)
        {
            throw new InvalidOperationException("No games played.");
        }
        
        Console.WriteLine(
            $"{"Game ID",-40}| {"1st player",-15}| {"2nd player",-15}| {"Rating change",-16}| {"Game type",-16}|");

        foreach (var game in GameHistory(gameRepository))
        {
            var player1Color = game.WinnerId == game.Player1Id ? ConsoleColor.Green : ConsoleColor.Red;
            var player2Color = game.WinnerId == game.Player2Id ? ConsoleColor.Green : ConsoleColor.Red;

            var ratingChange = RatingHistory(gameRepository)[game.GameId];

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
            Console.Write($"{GetSafePlayerName(playerService, repository, game.Player1Id),-15}");
            Console.ResetColor();

            Console.Write("| ");

            Console.ForegroundColor = player2Color;
            Console.Write($"{GetSafePlayerName(playerService, repository, game.Player2Id),-15}");
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
        Console.Write($"Games played: {GameHistory(gameRepository).Count} ");
        Console.WriteLine("|");
    }
}

public class StandardGameAccount(string userName, int currentRating)
    : GameAccount(userName, currentRating)
{
}

public class HalfLossGameAccount(string userName, int currentRating)
    : GameAccount(userName, currentRating)
{
    public override void LoseGame(IGameRepository gameRepository, Game game)
    {
        if (game.WinnerId == UserId)
        {
            throw new ArgumentException("Invalid game.");
        }

        var predictedRating = Math.Max(MinRating, CurrentRating + game.GetSignedRatingChange(this) / 2);
        if (predictedRating == MinRating)
        {
            RatingHistory(gameRepository)[game.GameId] = MinRating - CurrentRating;
            CurrentRating = MinRating;
        }
        else
        {
            RatingHistory(gameRepository)[game.GameId] = game.GetSignedRatingChange(this) / 2;
            CurrentRating = predictedRating;
        }

        GameHistory(gameRepository).Add(game);
    }
}

public class StreakBonusGameAccount(string userName, int currentRating)
    : GameAccount(userName, currentRating)
{
    private int WinStreak { get; set; }

    private Dictionary<Guid, int> WinStreakHistory(IGameRepository gameRepository)
    {
        return gameRepository.GetDbContext().WinStreakHistories[UserId];
    }

    public override void WinGame(IGameRepository gameRepository, Game game)
    {
        if (game.WinnerId != UserId)
        {
            throw new ArgumentException("Invalid game.");
        }

        if (!gameRepository.GetDbContext().WinStreakHistories.ContainsKey(UserId))
        {
            gameRepository.GetDbContext().WinStreakHistories[UserId] = [];
        }

        var bonus = game.GameType != GameType.Training ? WinStreak : 0;
        CurrentRating += game.GetSignedRatingChange(this) + bonus;
        RatingHistory(gameRepository)[game.GameId] = game.GetSignedRatingChange(this) + bonus;

        GameHistory(gameRepository).Add(game);
        UpdateWinStreak(gameRepository, game);
    }

    public override void LoseGame(IGameRepository gameRepository, Game game)
    {
        if (!gameRepository.GetDbContext().WinStreakHistories.ContainsKey(UserId))
        {
            gameRepository.GetDbContext().WinStreakHistories[UserId] = [];
        }

        base.LoseGame(gameRepository, game);
        UpdateWinStreak(gameRepository, game);
    }

    private void UpdateWinStreak(IGameRepository gameRepository, Game game)
    {
        var ratingChange = RatingHistory(gameRepository)[game.GameId];
        switch (ratingChange)
        {
            case > 0:
                WinStreak++;
                break;
            case < 0:
                WinStreak = 0;
                break;
        }

        WinStreakHistory(gameRepository)[game.GameId] = WinStreak;
    }

    public override void PrintStats(PlayerService playerService, IGameAccountRepository accountRepository,
        IGameRepository gameRepository)
    {
        Console.WriteLine(
            $"{"Game ID",-40}| {"1st player",-15}| {"2nd player",-15}| {"Rating change",-16}| {"Game type",-16}| {"Streak",-7}|");

        foreach (var game in GameHistory(gameRepository))
        {
            var player1Color = game.WinnerId == game.Player1Id ? ConsoleColor.Green : ConsoleColor.Red;
            var player2Color = game.WinnerId == game.Player2Id ? ConsoleColor.Green : ConsoleColor.Red;

            var ratingChange = RatingHistory(gameRepository)[game.GameId];

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
            Console.Write($"{GetSafePlayerName(playerService, accountRepository, game.Player1Id),-15}");
            Console.ResetColor();

            Console.Write("| ");

            Console.ForegroundColor = player2Color;
            Console.Write($"{GetSafePlayerName(playerService, accountRepository, game.Player2Id),-15}");
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


            if (WinStreakHistory(gameRepository).TryGetValue(game.GameId, out var winStreak))
            {
                Console.Write($"| {winStreak,-7}");
            }

            Console.WriteLine("|");
        }

        Console.Write($"{"| ",+76}");
        Console.Write($"Games played: {GameHistory(gameRepository).Count} ");
        Console.WriteLine("|");
    }
}