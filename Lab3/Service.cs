namespace Lab3;

public interface IPlayerService
{
    void CreateAccount(GameAccount account);
    IEnumerable<GameAccount> ReadAccounts();
    GameAccount? ReadAccountById(Guid userId);
    IEnumerable<Game> ReadPlayerGamesByPlayerId(Guid playerId, IGameRepository gameRepository);
    void DeleteAccount(Guid userId);
    void PrintAllPlayerStats(IGameRepository gameRepository);
    void DeleteAllPlayers();
}

public interface IGameService
{
    void CreateGame(Game game);
    IEnumerable<Game> ReadGames();
    void DeleteGame(Guid gameId);
    void ProcessAllGames(PlayerService playerService);
    void PrintAllGames();
    void DeleteAllGames();
}

public class PlayerService(IGameAccountRepository repository) : IPlayerService
{
    private readonly IGameAccountRepository _repository = repository;

    public void CreateAccount(GameAccount account)
    {
        _repository.Create(account);
    }

    public IEnumerable<GameAccount> ReadAccounts()
    {
        return _repository.ReadAll();
    }

    public GameAccount? ReadAccountById(Guid userId)
    {
        return _repository.Read(userId);
    }

    public IEnumerable<Game> ReadPlayerGamesByPlayerId(Guid playerId, IGameRepository gameRepository)
    {
        var account = _repository.Read(playerId);
        return account?.GameHistory(gameRepository) ?? Enumerable.Empty<Game>();
    }

    public void DeleteAccount(Guid userId)
    {
        _repository.Delete(userId);
    }

    public void PrintAllPlayerStats(IGameRepository gameRepository)
    {
        foreach (var player in ReadAccounts())
        {
            Console.WriteLine($"\n{player.UserName} ({player.GetType().Name}) Stats:");
            player.PrintStats(this, _repository, gameRepository);
            Console.WriteLine($"Current Rating of {player.UserName}, ID {player.UserId}: {player.CurrentRating}");
        }
    }

    public void DeleteAllPlayers()
    {
        foreach (var player in ReadAccounts())
        {
            DeleteAccount(player.UserId);
        }
    }
}

public class GameService(IGameRepository repository) : IGameService
{
    private readonly IGameRepository _repository = repository;

    public void CreateGame(Game game)
    {
        _repository.Create(game);
    }

    public IEnumerable<Game> ReadGames()
    {
        return _repository.ReadAll();
    }

    public void ProcessAllGames(PlayerService playerService)
    {
        foreach (var game in ReadGames())
        {
            game.ProcessGame(playerService, _repository);
        }
    }

    public void DeleteGame(Guid gameId)
    {
        _repository.Delete(gameId);
    }

    public void PrintAllGames()
    {
        Console.WriteLine("\nAll games:");
        foreach (var game in ReadGames())
        {
            Console.WriteLine(game.GameId);
        }
    }

    public void DeleteAllGames()
    {
        foreach (var game in ReadGames())
        {
            DeleteGame(game.GameId);
        }
    }
}
