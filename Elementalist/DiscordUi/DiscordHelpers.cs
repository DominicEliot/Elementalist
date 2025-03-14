using System.Text.RegularExpressions;
using Discord;
using Elementalist.Models;

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

    internal static Color GetCardColor(string elements)
    {
        if (elements.Contains(","))
            return new Color(0xb28950);

        if (elements.Contains("Fire"))
            return new Color(FireColor);

        if (elements.Contains("Air"))
            return new Color(AirColor);

        if (elements.Contains("Earth"))
            return new Color(EarthColor);

        if (elements.Contains("Water"))
            return new Color(WaterColor);

        // Colorless
        return new Color(ColorlessColor);
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

    public static string GetThresholdEmojis(Thresholds thresholds)
    {
        return string.Concat(Enumerable.Repeat(EarthEmoji, thresholds.Earth))
                + string.Concat(Enumerable.Repeat(FireEmoji, thresholds.Fire))
                + string.Concat(Enumerable.Repeat(WaterEmoji, thresholds.Water))
                + string.Concat(Enumerable.Repeat(AirEmoji, thresholds.Air));
    }

    public static string GetManaEmojis(Card card)
    {
        // Unfortunately curiosa's format doesn't have a good meta data around X spells
        if (Regex.IsMatch(card.Guardian.RulesText, @"\bX\b"))
        {
            return ManaXEmoji;
        }

        return card.Guardian.Cost switch
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
