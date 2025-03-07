using Discord;

namespace SorceryBot.DiscordUi;

public static class DiscordLookups
{
    internal static Color GetCardColor(string elements)
    {
        if (elements.Contains(","))
            return new Color(0xb28950);

        if (elements.Contains("Fire"))
            return new Color(0xfb671d);

        if (elements.Contains("Air"))
            return new Color(0x959cb8);

        if (elements.Contains("Earth"))
            return new Color(0x909090);

        if (elements.Contains("Water"))
            return new Color(0x19cce3);

        // Colorless
        return new Color(0xdcdcdc);
    }

    public static string ReplaceManaTokensWithEmojis(string input)
    {
        input = input.Replace("(A)", "<:t_air:1347668260157526046>");
        input = input.Replace("(W)", "<:t_water:1347668265463447693>");
        input = input.Replace("(E)", "<:t_earth:1347668261642440827>");
        input = input.Replace("(F)", "<:t_fire:1347668263806697472>");
        input = input.Replace("(0)", "<:m_00:1347668233116713093>");
        input = input.Replace("(1)", "<:m_01:1347668234614079569>");
        input = input.Replace("①", "<:m_01:1347668234614079569>");
        input = input.Replace("(2)", "<:m_02:1347668236824612985>");
        input = input.Replace("(3)", "<:m_03:1347668238418579638>");
        input = input.Replace("(4)", "<:m_04:1347668240364601354>");
        input = input.Replace("(X)", "<:m_X:1347668251500609546>");

        return input;
    }
}
