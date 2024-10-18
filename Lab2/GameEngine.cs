namespace Lab2;

public class GameEngine
{
    public static void ProcessAllGames()
    {
        foreach (var game in Game.Games.Values)
        {
            game.ProcessGame();
        }
    }
}