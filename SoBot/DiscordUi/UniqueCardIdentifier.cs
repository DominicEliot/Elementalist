namespace SorceryBot.DiscordUi;

public class UniqueCardIdentifier
{
    public string CardName { get; }
    public string Set { get; }
    public string Product { get; }
    public string Finish { get; }

    public UniqueCardIdentifier(string cardName, string set, string product, string finish)
    {
        CardName = cardName;
        Set = set;
        Product = product;
        Finish = finish;
    }

    public UniqueCardIdentifier(string cardName, SetVariant setVariant)
    {
        CardName = cardName;
        Set = setVariant.Set.Name;
        Product = setVariant.Variant.Product;
        Finish = setVariant.Variant.Finish;
    }

    public override string ToString() => $"{CardName} - {Set} - {Product} - {Finish}";

    public string ToString(string spacer) => $"{CardName}{spacer}{Set}{spacer}{Product}{spacer}{Finish}";

    public string ToNamelessString() => $"{Set} - {Product} - {Finish}";

    public string ToNamelessString(string spacer) => $"{Set}{spacer}{Product}{spacer}{Finish}";
}
