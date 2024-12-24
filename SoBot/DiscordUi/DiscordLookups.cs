using Discord;

namespace SorceryBot.DiscordUi;

internal static class DiscordLookups
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
}
