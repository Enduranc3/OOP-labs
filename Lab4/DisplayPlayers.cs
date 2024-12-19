namespace Lab4;

public class DisplayPlayers(IPlayerService playerService) : ICommand
{
    public void Execute()
    {
        var players = playerService.ReadAccounts();
        if (!players.Any())
        {
            Console.WriteLine("No players found.");
            Thread.Sleep(2000);
            return;
        }
        foreach (var player in players)
        {
            Console.WriteLine($"Player ID: {player.UserId}, Name: {player.UserName}, Rating: {player.CurrentRating}");
        }
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey();
    }

    public void DisplayCapabilities()
    {
        Console.WriteLine("Displays all players from the database.");
    }
}