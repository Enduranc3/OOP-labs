namespace Lab2;

internal static class Program
{
    private static void Main()
    {
        var player1 = new StandardGameAccount("Player1", new Random().Next(1, 5));
        var player2 = new HalfLossGameAccount("Player2", new Random().Next(1, 1000));
        var player3 = new StreakBonusGameAccount("Player3", new Random().Next(1, 1000));

        Console.WriteLine($"Current Rating of {player1.UserName}, ID {player1.UserId}: {player1.CurrentRating}");
        Console.WriteLine($"Current Rating of {player2.UserName}, ID {player2.UserId}: {player2.CurrentRating}");
        Console.WriteLine($"Current Rating of {player3.UserName}, ID {player3.UserId}: {player3.CurrentRating}\n");

        var random = new Random();
        GameFactory.CreateGame(GameType.Standard, player1.UserId, player2.UserId,
            random.Next(2) == 0 ? player1.UserId : player2.UserId, new Random().Next(1, 30));
        GameFactory.CreateGame(GameType.Training, player1.UserId, player2.UserId,
            random.Next(2) == 0 ? player1.UserId : player2.UserId);
        GameFactory.CreateGame(GameType.SingleRating, player2.UserId, player3.UserId,
            random.Next(2) == 0 ? player2.UserId : player3.UserId, new Random().Next(1, 30), player2.UserId);
        GameFactory.CreateGame(GameType.Standard, player2.UserId, player3.UserId,
            random.Next(2) == 0 ? player2.UserId : player3.UserId, new Random().Next(1, 30));
        GameFactory.CreateGame(GameType.SingleRating, player1.UserId, player3.UserId,
            random.Next(2) == 0 ? player1.UserId : player3.UserId, new Random().Next(1, 30), player3.UserId);
        GameFactory.CreateGame(GameType.Training, player3.UserId, player1.UserId,
            random.Next(2) == 0 ? player3.UserId : player1.UserId);
        GameFactory.CreateGame(GameType.Training, player3.UserId, player2.UserId,
            random.Next(2) == 0 ? player3.UserId : player2.UserId);
        GameFactory.CreateGame(GameType.Standard, player1.UserId, player3.UserId,
            random.Next(2) == 0 ? player1.UserId : player3.UserId, new Random().Next(1, 30));

        GameEngine.ProcessAllGames();

        Console.WriteLine($"Player1 ({player1.GetType().Name}) Stats:");
        player1.PrintStats();

        Console.WriteLine($"\nPlayer2 ({player2.GetType().Name}) Stats:");
        player2.PrintStats();

        Console.WriteLine($"\nPlayer3 ({player3.GetType().Name}) Stats:");
        player3.PrintStats();

        Console.WriteLine($"\nCurrent Rating of {player1.UserName}, ID {player1.UserId}: {player1.CurrentRating}");
        Console.WriteLine($"Current Rating of {player2.UserName}, ID {player2.UserId}: {player2.CurrentRating}");
        Console.WriteLine($"Current Rating of {player3.UserName}, ID {player3.UserId}: {player3.CurrentRating}");
    }
}