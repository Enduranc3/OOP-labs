namespace Lab4;

public interface IGameAccountRepository
{
    void Create(GameAccount account);
    GameAccount? Read(Guid userId);
    IEnumerable<GameAccount> ReadAll();
    void Update(GameAccount account);
    void Delete(Guid userId);
}

public interface IGameRepository
{
    void Create(Game game);
    Game? Read(Guid gameId);
    IEnumerable<Game> ReadAll();
    void Update(Game game);
    void Delete(Guid gameId);
    DbContext GetDbContext();
}

public class GameAccountRepository(DbContext dbContext) : IGameAccountRepository
{
    public void Create(GameAccount account)
    {
        dbContext.Accounts[account.UserId] = account;
        dbContext.RatingHistories[account.UserId] = [];
        dbContext.WinStreakHistories[account.UserId] = [];
    }

    public GameAccount? Read(Guid userId)
    {
        return dbContext.Accounts.GetValueOrDefault(userId);
    }

    public IEnumerable<GameAccount> ReadAll()
    {
        return dbContext.Accounts.Values;
    }

    public void Update(GameAccount account)
    {
        if (dbContext.Accounts.ContainsKey(account.UserId))
        {
            dbContext.Accounts[account.UserId] = account;
        }
    }

    public void Delete(Guid userId)
    {
        dbContext.Accounts.Remove(userId);
    }
}

public class GameRepository(DbContext dbContext) : IGameRepository
{
    private readonly DbContext _dbContext = dbContext;

    public void Create(Game game)
    {
        _dbContext.Games[game.GameId] = game;
    }

    public Game? Read(Guid gameId)
    {
        return _dbContext.Games.GetValueOrDefault(gameId);
    }

    public IEnumerable<Game> ReadAll()
    {
        return _dbContext.Games.Values;
    }

    public void Update(Game game)
    {
        if (_dbContext.Games.ContainsKey(game.GameId))
        {
            _dbContext.Games[game.GameId] = game;
        }
    }

    public void Delete(Guid gameId)
    {
        _dbContext.Games.Remove(gameId);
    }

    public DbContext GetDbContext()
    {
        return _dbContext;
    }
}