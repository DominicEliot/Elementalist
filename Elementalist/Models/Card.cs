using System.Text.Json.Serialization;

namespace Elementalist.Models;

public class Card
{
    [JsonConstructor]
    public Card(string name, Guardian guardian, string elements, string subTypes, IEnumerable<Set> sets)
    {
        Name = name;
        Guardian = guardian;
        Elements = elements;
        SubTypes = subTypes;
        Sets = sets;
    }

    public string Name { get; init; }
    public Guardian Guardian { get; init; }
    public string Elements { get; init; }
    public string SubTypes { get; init; }
    public IEnumerable<Set> Sets { get; init; }
}

public class Guardian
{
    [JsonConstructor]
    public Guardian(string rarity, string type, string rulesText, int? cost, int? attack, int? defence, int? life, Thresholds thresholds)
    {
        Rarity = rarity;
        Type = type;
        RulesText = rulesText;
        Cost = cost;
        Attack = attack;
        Defence = defence;
        Life = life;
        Thresholds = thresholds;
    }

    public string Rarity { get; init; }
    public string Type { get; init; }
    public string RulesText { get; init; }
    public int? Cost { get; init; }
    public int? Attack { get; init; }
    public int? Defence { get; init; }
    public int? Life { get; init; }
    public Thresholds Thresholds { get; init; }
}

public class Thresholds
{
    [JsonConstructor]
    public Thresholds(int air, int earth, int fire, int water)
    {
        Air = air;
        Earth = earth;
        Fire = fire;
        Water = water;
    }

    public int Air { get; init; }
    public int Earth { get; init; }
    public int Fire { get; init; }
    public int Water { get; init; }
}

public class Set
{
    [JsonConstructor]
    public Set(string name, DateTime releasedAt, Metadata metadata, IEnumerable<Variant> variants)
    {
        Name = name;
        ReleasedAt = releasedAt;
        Metadata = metadata;
        Variants = variants;
    }

    public string Name { get; init; }
    public DateTime ReleasedAt { get; init; }
    public Metadata Metadata { get; init; }
    public IEnumerable<Variant> Variants { get; init; }
}

public class Metadata
{
    [JsonConstructor]
    public Metadata(string rarity, string type, string rulesText, int? cost, int? attack, int? defence, int? life, Thresholds thresholds)
    {
        Rarity = rarity;
        Type = type;
        RulesText = rulesText;
        Cost = cost;
        Attack = attack;
        Defence = defence;
        Life = life;
        Thresholds = thresholds;
    }

    public string Rarity { get; init; }
    public string Type { get; init; }
    public string RulesText { get; init; }
    public int? Cost { get; init; }
    public int? Attack { get; init; }
    public int? Defence { get; init; }
    public int? Life { get; init; }
    public Thresholds Thresholds { get; init; }
}

public class SetVariant
{
    public required Set Set { get; init; }
    public required Variant Variant { get; init; }
}

public class Variant
{
    [JsonConstructor]
    public Variant(string slug, string finish, string product, string artist, string flavorText, string typeText)
    {
        Slug = slug;
        Finish = finish;
        Product = product;
        Artist = artist;
        FlavorText = flavorText;
        TypeText = typeText;
    }

    public string Slug { get; init; }
    public string Finish { get; init; }
    public string Product { get; init; }
    public string Artist { get; init; }
    public string FlavorText { get; init; }
    public string TypeText { get; init; }
}
