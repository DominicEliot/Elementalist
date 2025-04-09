using System.Text.Json.Serialization;

namespace Elementalist.Models;

public class TcgPlayerSet
{
    [JsonConstructor]
    public TcgPlayerSet(int id, string name, int count, int total, List<TcgPlayerCard> result)
    {
        Id = id;
        Name = name;
        Count = count;
        Total = total;
        Result = result;
    }

    public int Id { get; init; }
    public string Name { get; init; }
    public int Count { get; init; }
    public int Total { get; init; }
    public List<TcgPlayerCard> Result { get; init; }
}

public class TcgPlayerCard
{
    private string _printing;

    [JsonConstructor]
    public TcgPlayerCard(int productID, int productConditionID, string? condition, string? game, bool isSupplemental, float lowPrice, float marketPrice, string? number, string printing, string productName, string? rarity, int sales, string? set, string? setAbbrv, string? type)
    {
        ProductID = productID;
        ProductConditionID = productConditionID;
        Condition = condition;
        Game = game;
        IsSupplemental = isSupplemental;
        LowPrice = lowPrice;
        MarketPrice = marketPrice;
        Number = number;
        Printing = (printing == "Normal") ? "Standard" : printing;
        ProductName = productName;
        Rarity = rarity;
        Sales = sales;
        Set = set;
        SetAbbrv = setAbbrv;
        Type = type;
    }

    public int ProductID { get; init; }
    public int ProductConditionID { get; init; }
    public string? Condition { get; init; }
    public string? Game { get; init; }
    public bool IsSupplemental { get; init; }
    public float LowPrice { get; init; }
    public float MarketPrice { get; init; }
    public string? Number { get; init; }
    public string Printing { get; init; }
    public string ProductName { get; init; }
    public string? Rarity { get; init; }
    public int Sales { get; init; }
    public string? Set { get; init; }
    public string? SetAbbrv { get; init; }
    public string? Type { get; init; }
}

public class TcgPlayerSetResult
{
    [JsonConstructor]
    public TcgPlayerSetResult(int setNameId, int categoryId, string name, string cleanSetName, string urlName, string abbreviation, DateTime releaseDate, bool isSupplemental, bool active)
    {
        SetNameId = setNameId;
        CategoryId = categoryId;
        Name = name;
        CleanSetName = cleanSetName;
        UrlName = urlName;
        Abbreviation = abbreviation;
        ReleaseDate = releaseDate;
        IsSupplemental = isSupplemental;
        Active = active;
    }

    public int SetNameId { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; }
    public string CleanSetName { get; init; }
    public string UrlName { get; init; }
    public string Abbreviation { get; init; }
    public DateTime ReleaseDate { get; init; }
    public bool IsSupplemental { get; init; }
    public bool Active { get; init; }
}

public class TcgPlayerGameSets
{
    [JsonConstructor]
    public TcgPlayerGameSets(List<object> errors, List<TcgPlayerSetResult> results)
    {
        Errors = errors;
        Results = results;
    }

    public List<object> Errors { get; init; }
    public List<TcgPlayerSetResult> Results { get; init; }
}
