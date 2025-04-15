using ElementalistBot.DiscordUi;
using Xunit;

namespace SorceryBotTests.DiscordUi;

public class HelpCommandTests
{
    [Fact]
    public void HelpCommandTest()
    {
        var command = new HelpCommand();

        var helpText = command.ShowHelpText();

        Assert.Contains("faq", helpText, StringComparison.OrdinalIgnoreCase);
    }
}
