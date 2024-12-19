namespace Lab4;

public class DisplayPlayersStats(IPlayerService playerService, IGameRepository gameRepository)
    : ICommand
{
    public void Execute()
    {
        try
        {
            playerService.PrintAllPlayerStats(gameRepository);
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            Thread.Sleep(2000);
        }

    }

    public void DisplayCapabilities()
    {
        Console.WriteLine("Displays stats for all players.");
    }
}