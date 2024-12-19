namespace Lab4;

public class AddPlayer(IPlayerService playerService) : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Enter player name:");
        var name = Console.ReadLine();
        Console.WriteLine("Enter initial rating:");
        var rating = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
        Console.WriteLine("Enter account type (Standard, HalfLoss, StreakBonus):");
        var accountType = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(accountType))
        {
            throw new InvalidOperationException("Account type cannot be empty.");
        }

        GameAccount account = accountType.ToLower() switch
        {
            "standard" => new StandardGameAccount(name ?? throw new InvalidOperationException(), rating),
            "halfloss" => new HalfLossGameAccount(name ?? throw new InvalidOperationException(), rating),
            "streakbonus" => new StreakBonusGameAccount(name ?? throw new InvalidOperationException(), rating),
            _ => throw new ArgumentException("Invalid account type.")
        };

        playerService.CreateAccount(account);
        Console.WriteLine("Player added successfully.");
    }

    public void DisplayCapabilities()
    {
        Console.WriteLine("Adds a new player with a specified account type.");
    }
}