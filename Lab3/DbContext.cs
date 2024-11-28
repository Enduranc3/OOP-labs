namespace Lab3;

public class DbContext
{
    public Dictionary<Guid, GameAccount> Accounts { get; } = [];
    public Dictionary<Guid, Game> Games { get; } = [];

    public Dictionary<Guid, Dictionary<Guid, int>> RatingHistories { get; } = [];

    public Dictionary<Guid, Dictionary<Guid, int>> WinStreakHistories { get; } = [];
}
