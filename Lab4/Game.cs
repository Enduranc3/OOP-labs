namespace Lab4;

public abstract class Game(
    Guid player1Id,
    Guid player2Id,
    Guid winnerId,
    int ratingChange,
    GameType gameType)
{
    public Guid GameId { get; } = Guid.NewGuid();
    public Guid Player1Id { get; } = player1Id;
    public Guid Player2Id { get; } = player2Id;
    public Guid WinnerId { get; } = winnerId;
    protected int RatingChange { get; } = ratingChange;
    public GameType GameType { get; } = gameType;

    protected virtual void ValidateGame()
    {
        if (Player1Id == Guid.Empty || Player2Id == Guid.Empty || WinnerId == Guid.Empty)
        {
            throw new ArgumentException("Player1Id, Player2Id, and WinnerId cannot be empty.");
        }

        if (RatingChange < 0)
        {
            throw new ArgumentException("RatingChange cannot be negative.");
        }

        if (Player1Id == Player2Id)
        {
            throw new ArgumentException("Player1Id and Player2Id cannot be the same.");
        }

        if (WinnerId != Player1Id && WinnerId != Player2Id)
        {
            throw new ArgumentException("WinnerId must be either Player1Id or Player2Id.");
        }
    }

    public void ProcessGame(PlayerService playerService, IGameRepository gameRepository)
    {
        ValidateGame();
        var player1 = playerService.ReadAccountById(Player1Id);
        var player2 = playerService.ReadAccountById(Player2Id);

        if (player1 == null || player2 == null)
        {
            throw new ArgumentException("Player1Id and Player2Id must be valid user IDs.");
        }

        if (player1.UserId != WinnerId)
        {
            player1.LoseGame(gameRepository, this);
            player2.WinGame(gameRepository, this);
            return;
        }

        player1.WinGame(gameRepository, this);
        player2.LoseGame(gameRepository, this);
    }

    public virtual int GetSignedRatingChange(GameAccount player)
    {
        return WinnerId == player.UserId ? RatingChange : -RatingChange;
    }
}

public class StandardGame(Guid player1Id, Guid player2Id, Guid winnerId, int ratingChange)
    : Game(player1Id, player2Id, winnerId, ratingChange, GameType.Standard)
{
    protected override void ValidateGame()
    {
        base.ValidateGame();
        if (RatingChange == 0)
        {
            throw new ArgumentException("RatingChange must be a positive integer for Standard games.");
        }
    }
}

public class TrainingGame(Guid player1Id, Guid player2Id, Guid winnerId, int ratingChange = 0)
    : Game(player1Id, player2Id, winnerId, ratingChange, GameType.Training)
{
    protected override void ValidateGame()
    {
        base.ValidateGame();
        if (RatingChange != 0)
        {
            throw new ArgumentException("RatingChange must be 0 for Training games.");
        }
    }
}

public class SingleRatingGame(
    Guid player1Id,
    Guid player2Id,
    Guid winnerId,
    int ratingChange,
    Guid changedPlayerId) : Game(player1Id, player2Id, winnerId, ratingChange, GameType.SingleRating)
{
    protected override void ValidateGame()
    {
        base.ValidateGame();
        if (RatingChange == 0)
        {
            throw new ArgumentException("RatingChange must be a positive integer for SingleRating games.");
        }

        if (changedPlayerId != Player1Id && changedPlayerId != Player2Id)
        {
            throw new ArgumentException("ChangedPlayerId must be either Player1Id or Player2Id.");
        }
    }

    public override int GetSignedRatingChange(GameAccount player)
    {
        if (player.UserId != changedPlayerId)
        {
            return 0;
        }

        return WinnerId == player.UserId ? RatingChange : -RatingChange;
    }
}