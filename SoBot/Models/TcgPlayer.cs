namespace SorceryBot.Models;

public class TcgPlayerSet
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int Total { get; set; }
    public List<TcgPlayerCard> Result { get; set; } = [];
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

public class TcgPlayerSetResult
{
    public int SetNameId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string CleanSetName { get; set; }
    public string UrlName { get; set; }
    public string Abbreviation { get; set; }
    public DateTime ReleaseDate { get; set; }
    public bool IsSupplemental { get; set; }
    public bool Active { get; set; }
}

public class TcgPlayerGameSets
{
    public List<object> Errors { get; set; } = [];
    public List<TcgPlayerSetResult> Results { get; set; } = [];
}
