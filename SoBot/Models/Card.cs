namespace SorceryBot.Models;

public class Card
{
    public string Name { get; set; }
    public Guardian Guardian { get; set; }
    public string Elements { get; set; }
    public string SubTypes { get; set; }
    public IEnumerable<Set> Sets { get; set; }
}

public class Guardian
{
    public string Rarity { get; set; }
    public string Type { get; set; }
    public string RulesText { get; set; }
    public int? Cost { get; set; }
    public int? Attack { get; set; }
    public int? Defence { get; set; }
    public int? Life { get; set; }
    public Thresholds Thresholds { get; set; }
}

public class Thresholds
{
    public int Air { get; set; }
    public int Earth { get; set; }
    public int Fire { get; set; }
    public int Water { get; set; }
}

public class Set
{
    public string Name { get; set; }
    public DateTime ReleasedAt { get; set; }
    public Metadata Metadata { get; set; }
    public IEnumerable<Variant> Variants { get; set; }
}

public class Metadata
{
    public string Rarity { get; set; }
    public string Type { get; set; }
    public string RulesText { get; set; }
    public int? Cost { get; set; }
    public int? Attack { get; set; }
    public int? Defence { get; set; }
    public int? Life { get; set; }
    public Thresholds Thresholds { get; set; }
}

public class Variant
{
    public string Slug { get; set; }
    public string Finish { get; set; }
    public string Product { get; set; }
    public string Artist { get; set; }
    public string FlavorText { get; set; }
    public string TypeText { get; set; }
}
