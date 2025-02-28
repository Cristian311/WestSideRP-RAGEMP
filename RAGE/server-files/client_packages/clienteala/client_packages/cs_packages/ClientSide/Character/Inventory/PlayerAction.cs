public class PlayerAction
{
    // Properties
    public string Action { get; set; }
    public int ItemId { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int Amount { get; set; }

    // Constructor
    public PlayerAction(string action, int itemId, int playerId, string playerName, int amount)
    {
        Action = action;
        ItemId = itemId;
        PlayerId = playerId;
        PlayerName = playerName;
        Amount = amount;
    }
}
