using System.Text.Json.Serialization;

namespace Elementalist.Models;

/// <summary>
/// The JSON shape returned by GET /api/cards.
/// </summary>
public sealed record Card(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("engine")] CardEngine Engine,
    [property: JsonPropertyName("printings")]
    IReadOnlyList<CardPrinting> Printings
);

public sealed record CardEngine(
    [property: JsonPropertyName("type")] CardType Type,
    [property: JsonPropertyName("category")]
    CardCategory Category,
    [property: JsonPropertyName("rarity")] RaritySlot? Rarity,
    [property: JsonPropertyName("slot")] RaritySlot? Slot,
    [property: JsonPropertyName("rules")] string? Rules,
    [property: JsonPropertyName("cost")] int? Cost,
    [property: JsonPropertyName("attack")] int? Attack,
    [property: JsonPropertyName("defense")]
    int? Defense,
    [property: JsonPropertyName("life")] int? Life,
    [property: JsonPropertyName("water")] int? Water,
    [property: JsonPropertyName("earth")] int? Earth,
    [property: JsonPropertyName("fire")] int? Fire,
    [property: JsonPropertyName("air")] int? Air,
    [property: JsonPropertyName("elements")]
    IReadOnlyList<Element> Elements,
    [property: JsonPropertyName("subtypes")]
    IReadOnlyList<string> Subtypes,
    [property: JsonPropertyName("keywords")]
    IReadOnlyList<string> Keywords,
    [property: JsonPropertyName("umbrellas")]
    IReadOnlyList<string> Umbrellas,
    [property: JsonPropertyName("back")] CardEngineBack? Back
);

/// <summary>
/// Mirrors CardEngine but without a nested "back"
/// </summary>
public sealed record CardEngineBack(
    [property: JsonPropertyName("type")] CardType Type,
    [property: JsonPropertyName("category")]
    CardCategory Category,
    [property: JsonPropertyName("rarity")] RaritySlot? Rarity,
    [property: JsonPropertyName("slot")] RaritySlot? Slot,
    [property: JsonPropertyName("rules")] string? Rules,
    [property: JsonPropertyName("cost")] int? Cost,
    [property: JsonPropertyName("attack")] int? Attack,
    [property: JsonPropertyName("defense")]
    int? Defense,
    [property: JsonPropertyName("life")] int? Life,
    [property: JsonPropertyName("water")] int? Water,
    [property: JsonPropertyName("earth")] int? Earth,
    [property: JsonPropertyName("fire")] int? Fire,
    [property: JsonPropertyName("air")] int? Air,
    [property: JsonPropertyName("elements")]
    IReadOnlyList<Element> Elements,
    [property: JsonPropertyName("subtypes")]
    IReadOnlyList<string> Subtypes,
    [property: JsonPropertyName("keywords")]
    IReadOnlyList<string> Keywords,
    [property: JsonPropertyName("umbrellas")]
    IReadOnlyList<string> Umbrellas
);

public sealed record CardPrinting(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("printedAt")]
    DateTimeOffset PrintedAt,
    [property: JsonPropertyName("set")] CardSet Set,
    [property: JsonPropertyName("meta")] CardPrintingMeta Meta
);

public sealed record CardSet(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("releasedAt")]
    DateTimeOffset ReleasedAt
);

public sealed record CardPrintingMeta(
    [property: JsonPropertyName("finish")] Finish Finish,
    [property: JsonPropertyName("product")]
    string Product,
    [property: JsonPropertyName("typeline")]
    string Typeline,
    [property: JsonPropertyName("flavor")] string? Flavor,
    [property: JsonPropertyName("artist")] CardArtist Artist,
    [property: JsonPropertyName("back")] CardPrintingMetaBack? Back
);

/// <summary>
/// Mirrors CardPrintingMeta but without a nested "back".
/// </summary>
public sealed record CardPrintingMetaBack(
    [property: JsonPropertyName("finish")] Finish Finish,
    [property: JsonPropertyName("product")]
    string Product,
    [property: JsonPropertyName("typeline")]
    string Typeline,
    [property: JsonPropertyName("flavor")] string? Flavor,
    [property: JsonPropertyName("artist")] CardArtist Artist
);

public sealed record CardArtist(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("slug")] string Slug
);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardType
{
    Avatar,
    Minion,
    Magic,
    Aura,
    Artifact,
    Site
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardCategory
{
    Avatar,
    Spell,
    Site,
    Token
}

/// <summary>Shared by both "rarity" and "slot" in the original TS union.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RaritySlot
{
    Unique,
    Elite,
    Exceptional,
    Ordinary
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Element
{
    Earth,
    Fire,
    Water,
    Air,
    None
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Finish
{
    Standard,
    Foil,
    Rainbow
}
