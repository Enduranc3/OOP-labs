namespace Lab3;

internal static class Program
{
    private static void Main()
    {
        var dbContext = new DbContext();
        var gameAccountRepository = new GameAccountRepository(dbContext);
        var gameRepository = new GameRepository(dbContext);
        var playerService = new PlayerService(gameAccountRepository);
        var gameService = new GameService(gameRepository);

        CreateAndDisplayPlayers(gameAccountRepository, playerService);
        CreateAndDisplayGames(gameService,
            playerService.ReadAccounts().ToList(), new Random());

        gameService.ProcessAllGames(playerService);

        playerService.PrintAllPlayerStats(gameRepository);
        gameService.PrintAllGames();

        gameService.DeleteAllGames();
        playerService.DeleteAllPlayers();
    }

    private static void CreateAndDisplayPlayers(IGameAccountRepository repository,
        PlayerService playerService)
    {
        playerService.CreateAccount(new StandardGameAccount("Player1", new Random().Next(1, 5)));
        Console.WriteLine(
            $"Player1 initial rating: {playerService.ReadAccountById(repository.ReadAll().First().UserId)?.CurrentRating}");
        playerService.CreateAccount(new HalfLossGameAccount("Player2", new Random().Next(1, 1000)));
        Console.WriteLine(
            $"Player2 initial rating: {playerService.ReadAccountById(repository.ReadAll().Last().UserId)?.CurrentRating}");
        playerService.CreateAccount(new StreakBonusGameAccount("Player3", new Random().Next(1, 1000)));
        Console.WriteLine(
            $"Player3 initial rating: {playerService.ReadAccountById(repository.ReadAll().Last().UserId)?.CurrentRating}");
    }

    private static void CreateAndDisplayGames(GameService gameService,
        List<GameAccount> players, Random random)
    {
        try
        {
            gameService.CreateGame(GameFactory.CreateGame(GameType.Standard, players[0].UserId,
                players[1].UserId, random.Next(2) == 0 ? players[0].UserId : players[1].UserId,
                new Random().Next(1, 30)));

            gameService.CreateGame(GameFactory.CreateGame(GameType.Training, players[0].UserId,
                players[1].UserId,
                random.Next(2) == 0 ? players[0].UserId : players[1].UserId));

            gameService.CreateGame(GameFactory.CreateGame(GameType.SingleRating,
                players[1].UserId,
                players[2].UserId,
                random.Next(2) == 0 ? players[1].UserId : players[2].UserId, new Random().Next(1, 30),
                players[1].UserId));

            gameService.CreateGame(GameFactory.CreateGame(GameType.Standard, players[1].UserId,
                players[2].UserId,
                random.Next(2) == 0 ? players[1].UserId : players[2].UserId, new Random().Next(1, 30)));

            gameService.CreateGame(GameFactory.CreateGame(GameType.SingleRating,
                players[0].UserId,
                players[2].UserId,
                random.Next(2) == 0 ? players[0].UserId : players[2].UserId, new Random().Next(1, 30),
                players[2].UserId));

            gameService.CreateGame(GameFactory.CreateGame(GameType.Training, players[2].UserId,
                players[0].UserId,
                random.Next(2) == 0 ? players[2].UserId : players[0].UserId));

            gameService.CreateGame(GameFactory.CreateGame(GameType.Training, players[2].UserId,
                players[1].UserId,
                random.Next(2) == 0 ? players[2].UserId : players[1].UserId));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating games: {ex.Message}");
        }
    }
}
