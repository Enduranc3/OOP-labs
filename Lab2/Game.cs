namespace Lab2
{
    public abstract class Game
    {
        public static readonly Dictionary<Guid, Game> Games = new();
        public Guid GameId { get; }
        public Guid Player1Id { get; }
        public Guid Player2Id { get; }
        public Guid WinnerId { get; }
        protected int RatingChange { get; }
        public GameType GameType { get; }

        protected Game(Guid player1Id, Guid player2Id, Guid winnerId, int ratingChange, GameType gameType)
        {
            GameId = Guid.NewGuid();
            Player1Id = player1Id;
            Player2Id = player2Id;
            WinnerId = winnerId;
            RatingChange = ratingChange;
            GameType = gameType;
            Games[GameId] = this;
        }
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
        private void ProcessGame()
        {
            ValidateGame();
            var player1 = GameAccount.GetAccountById(Player1Id);
            var player2 = GameAccount.GetAccountById(Player2Id);

            if (player1.UserId != WinnerId)
            {
                player1.LoseGame(this);
                player2.WinGame(this);
                return;
            }

            player1.WinGame(this);
            player2.LoseGame(this);
        }
        public virtual int GetSignedRatingChange(GameAccount player)
        {
            return WinnerId == player.UserId ? RatingChange : -RatingChange;
        }
        public static void ProcessAllGames()
        {
            foreach (var game in Game.Games.Values)
            {
                game.ProcessGame();
            }
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

    public class SingleRatingGame(Guid player1Id, Guid player2Id, Guid winnerId, int ratingChange, Guid changedPlayerId)
        : Game(player1Id, player2Id, winnerId, ratingChange, GameType.SingleRating)
    {
        private Guid ChangedPlayerId { get; } = changedPlayerId;
        protected override void ValidateGame()
        {
            base.ValidateGame();
            if (RatingChange == 0)
            {
                throw new ArgumentException("RatingChange must be a positive integer for SingleRating games.");
            }
            if (ChangedPlayerId != Player1Id && ChangedPlayerId != Player2Id)
            {
                throw new ArgumentException("ChangedPlayerId must be either Player1Id or Player2Id.");
            }
        }

        public override int GetSignedRatingChange(GameAccount player)
        {
            if (player.UserId != ChangedPlayerId)
            {
                return 0;
            }

            return WinnerId == player.UserId ? RatingChange : -RatingChange;
        }
    }
}