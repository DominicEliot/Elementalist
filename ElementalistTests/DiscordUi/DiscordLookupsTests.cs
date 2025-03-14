using Elementalist.DiscordUi;
using Xunit;

namespace SorceryBotTests.DiscordUi;

public class DiscordLookupsTests
{
    [Fact]
    public void StringTokenToEmojiLeaveOtherTextTest()
    {
        var extraText = " - Some Rules Text";
        var earthToken = "(E)";
        var testString = $"{earthToken}{earthToken}{extraText}";

        var convertedText = DiscordHelpers.ReplaceManaTokensWithEmojis(testString);

        Assert.Contains(extraText, convertedText);
    }

    [Fact]
    public void StringTokenUnicodeReplacement()
    {
        var unicodeToken = "①";
        var convertedText = DiscordHelpers.ReplaceManaTokensWithEmojis(unicodeToken);
        Assert.DoesNotContain(unicodeToken, convertedText);
    }

    [Fact]
    public void StringTokenToEmojiReplaceWithEmojiTest()
    {
        var extraText = " - Some Rules Text";
        var earthToken = "(E)";
        var waterToken = "(W)";
        var airToken = "(A)";
        var fireToken = "(F)";
        var manaToken = "(1)";
        var testString = $"{earthToken}{waterToken}{airToken}{fireToken}{manaToken}{extraText}";

        var convertedText = DiscordHelpers.ReplaceManaTokensWithEmojis(testString);

        Assert.DoesNotContain(earthToken, convertedText);
        Assert.DoesNotContain(waterToken, convertedText);
        Assert.DoesNotContain(airToken, convertedText);
        Assert.DoesNotContain(fireToken, convertedText);
        Assert.DoesNotContain(manaToken, convertedText);
    }
}
