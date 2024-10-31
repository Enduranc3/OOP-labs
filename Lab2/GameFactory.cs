namespace Lab2;

public static class GameFactory
{
    public static Game CreateGame(GameType gameType, Guid player1Id, Guid player2Id, Guid winnerId,
        int ratingChange = 0, Guid changedPlayerId = default)
    {
        return gameType switch
        {
            GameType.Standard => new StandardGame(player1Id, player2Id, winnerId, ratingChange),
            GameType.Training => new TrainingGame(player1Id, player2Id, winnerId),
            GameType.SingleRating =>
                new SingleRatingGame(player1Id, player2Id, winnerId, ratingChange, changedPlayerId),
            _ => throw new ArgumentException("Invalid GameType.")
        };
    }
}