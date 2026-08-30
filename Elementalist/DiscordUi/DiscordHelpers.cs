using System.Text.RegularExpressions;
using Elementalist.Models;
using NetCord;

namespace Elementalist.DiscordUi;

public static class DiscordHelpers
{
    public const string AirEmoji = "<:t_air:1347746759153160254>";
    public const string WaterEmoji = "<:t_water:1347746830762512404>";
    public const string EarthEmoji = "<:t_earth:1347746782385274882>";
    public const string FireEmoji = "<:t_fire:1347746808712921138>";
    public const string Mana0Emoji = "<:m_00:1347746505179791482>";
    public const string Mana1Emoji = "<:m_01:1347746530756526205>";
    public const string Mana2Emoji = "<:m_02:1347746546908921928>";
    public const string Mana3Emoji = "<:m_03:1347746568966766683>";
    public const string Mana4Emoji = "<:m_04:1347746589141110885>";
    public const string Mana5Emoji = "<:m_05:1347746609311649903>";
    public const string Mana6Emoji = "<:m_06:1347746634708029511>";
    public const string Mana7Emoji = "<:m_07:1347746661031477359>";
    public const string Mana8Emoji = "<:m_08:1347746687648661524>";
    public const string Mana9Emoji = "<:m_09:1347746712604770358>";
    public const string ManaXEmoji = "<:m_X:1347746735723647147>";

    public const int FireColor = 0xfb671d;
    public const int AirColor = 0x959cb8;
    public const int EarthColor = 0x909090;
    public const int WaterColor = 0x19cce3;
    public const int ColorlessColor = 0xdcdcdc;

    internal static Color GetCardColor(IEnumerable<Element> elements)
    {
        var elementList = elements as IReadOnlyList<Element> ?? elements.ToList();

        if (elementList.Count > 1)
            return new Color(0xb28950);

        var element = elementList.FirstOrDefault();
        return element switch
        {
            Element.Fire => new Color(FireColor),
            Element.Air => new Color(AirColor),
            Element.Earth => new Color(EarthColor),
            Element.Water => new Color(WaterColor),
            _ => new Color(ColorlessColor)
        };
    }

    public static string ReplaceManaTokensWithEmojis(string input)
    {
        input = input.Replace("(A)", AirEmoji);
        input = input.Replace("(W)", WaterEmoji);
        input = input.Replace("(E)", EarthEmoji);
        input = input.Replace("(F)", FireEmoji);
        input = input.Replace("(0)", Mana0Emoji);
        input = input.Replace("(1)", Mana1Emoji);
        input = input.Replace("①", Mana1Emoji);
        input = input.Replace("(2)", Mana2Emoji);
        input = input.Replace("(3)", Mana3Emoji);
        input = input.Replace("(4)", Mana4Emoji);
        input = input.Replace("(X)", ManaXEmoji);

        return input;
    }

    public static string GetThresholdEmojis(CardEngine cardEngine)
    {
        return string.Concat(Enumerable.Repeat(EarthEmoji, cardEngine.Earth ?? 0))
               + string.Concat(Enumerable.Repeat(FireEmoji, cardEngine.Fire ?? 0))
               + string.Concat(Enumerable.Repeat(WaterEmoji, cardEngine.Water ?? 0))
               + string.Concat(Enumerable.Repeat(AirEmoji, cardEngine.Air ?? 0));
    }

    public static string GetManaEmojis(Card card)
    {
        // Unfortunately curiosa's format doesn't have a good meta data around X spells
        if (card.Engine.Rules is not null && Regex.IsMatch(card.Engine.Rules, @"\bX\b"))
        {
            return ManaXEmoji;
        }

        return card.Engine.Cost switch
        {
            0 => Mana0Emoji,
            1 => Mana1Emoji,
            2 => Mana2Emoji,
            3 => Mana3Emoji,
            4 => Mana4Emoji,
            5 => Mana5Emoji,
            6 => Mana6Emoji,
            7 => Mana7Emoji,
            8 => Mana8Emoji,
            9 => Mana9Emoji,
            _ => string.Empty
        };
    }
}
