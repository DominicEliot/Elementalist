using System.Text.Json;
using System.Text.Json.Serialization;

namespace SorceryBot.DiscordUi;

public class UniqueCardIdentifier
{
    [JsonPropertyName("C")]
    public string Name { get; }

    [JsonPropertyName("S")]
    public string Set { get; }

    [JsonPropertyName("P")]
    public string Product { get; }

    [JsonPropertyName("F")]
    public string Finish { get; }

    [JsonConstructor]
    public UniqueCardIdentifier(string cardName, string set, string product, string finish)
    {
        Name = cardName;
        Set = set;
        Product = product;
        Finish = finish;
    }

    public UniqueCardIdentifier(string cardName, SetVariant setVariant)
    {
        Name = cardName;
        Set = setVariant.Set.Name;
        Product = setVariant.Variant.Product;
        Finish = setVariant.Variant.Finish;
    }

    public override string ToString() => $"{Name} - {Set} - {Product} - {Finish}";

    public string ToString(string spacer) => $"{Name}{spacer}{Set}{spacer}{Product}{spacer}{Finish}";

    public string ToNamelessString() => $"{Set} - {Product} - {Finish}";

    public string ToNamelessString(string spacer) => $"{Set}{spacer}{Product}{spacer}{Finish}";

    public string ToJson(JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        return JsonSerializer.Serialize(this, options);
    }
}
