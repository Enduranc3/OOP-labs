namespace Lab4;

public class DisplayCertainPlayerStats(
    PlayerService playerService,
    IGameAccountRepository accountRepository,
    IGameRepository gameRepository) : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Enter player ID:");
        var playerIdInput = Console.ReadLine();
        if (Guid.TryParse(playerIdInput, out var playerId))
        {
            var player = playerService.ReadAccountById(playerId);
            if (player != null)
            {
                Console.WriteLine($"\n{player.UserName} ({player.GetType().Name}) Stats:");
                player.PrintStats(playerService, accountRepository, gameRepository);
                Console.WriteLine($"Current Rating of {player.UserName}, ID {player.UserId}: {player.CurrentRating}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Player not found.");
                Thread.Sleep(2000);
            }
        }
        else
        {
            Console.WriteLine("Invalid player ID.");
            Thread.Sleep(2000);
        }
    }

    public void DisplayCapabilities()
    {
        Console.WriteLine("Displays stats for a specific player.");
    }
}