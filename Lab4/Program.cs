namespace Lab4;

internal static class Program
{
    private static void Main()
    {
        var dbContext = new DbContext();
        var gameAccountRepository = new GameAccountRepository(dbContext);
        var gameRepository = new GameRepository(dbContext);
        var playerService = new PlayerService(gameAccountRepository);
        var gameService = new GameService(gameRepository);

        var commands = new List<ICommand>
        {
            new DisplayPlayers(playerService),
            new AddPlayer(playerService),
            new DisplayPlayersStats(playerService, gameRepository),
            new PlayGame(gameService, playerService),
            new DisplayCertainPlayerStats(playerService, gameAccountRepository, gameRepository)
        };

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Enter command number to execute (or 'exit' to quit):");
            DisplayAllCapabilities(commands);
            var input = Console.ReadLine();
            if (input?.ToLower() == "exit")
            {
                break;
            }

            if (int.TryParse(input, out var commandIndex) && commandIndex >= 0 && commandIndex < commands.Count)
            {
                commands[commandIndex].Execute();
            }
            else
            {
                Console.WriteLine("Invalid command number.");
            }
        }
    }

    private static void DisplayAllCapabilities(IEnumerable<ICommand> commands)
    {
        var index = 0;
        foreach (var command in commands)
        {
            Console.Write($"{index++}: ");
            command.DisplayCapabilities();
        }
    }
}