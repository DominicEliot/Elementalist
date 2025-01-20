namespace SorceryBot.Models;

public class TcgPlayerSet
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<TcgPlayerCard> Cards { get; set; } = [];
}

public class TcgPlayerCard
{
    public int ProductID { get; set; }
    public int ProductConditionID { get; set; }
    public string? Condition { get; set; }
    public string? Game { get; set; }
    public bool IsSupplemental { get; set; }
    public float LowPrice { get; set; }
    public float MarketPrice { get; set; }
    public string? Number { get; set; }
    public string Printing { get; set; }
    public string ProductName { get; set; }
    public string? Rarity { get; set; }
    public int Sales { get; set; }
    public string? Set { get; set; }
    public string? SetAbbrv { get; set; }
    public string? Type { get; set; }
}
