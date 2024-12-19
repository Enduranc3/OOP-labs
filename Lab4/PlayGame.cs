namespace Lab4;

public class PlayGame(IGameService gameService, IPlayerService playerService) : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Enter player 1 ID:");
        var player1Id = Guid.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
        Console.WriteLine("Enter player 2 ID:");
        var player2Id = Guid.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
        Console.WriteLine("Enter winner ID:");
        var winnerId = Guid.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
        Console.WriteLine("Enter game type (Standard, Training, SingleRating):");
        var gameType = Enum.Parse<GameType>(Console.ReadLine() ?? throw new InvalidOperationException());
        Console.WriteLine("Enter rating change (if applicable):");
        var ratingChange = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

        var game = GameFactory.CreateGame(gameType, player1Id, player2Id, winnerId, ratingChange);
        gameService.CreateGame(game);
        gameService.ProcessAllGames((PlayerService)playerService);
        Console.WriteLine("Game played successfully.");
    }

    public void DisplayCapabilities()
    {
        Console.WriteLine("Plays game and updates player stats.");
    }
}