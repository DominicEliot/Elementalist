using NSubstitute;
using ElementalistBot.DiscordUi;
using Xunit;
using Discord.Interactions;

namespace ElementalistTests.Features;
public class HelpTests
{
    [Fact]
    public void CheckAssemblyInfo()
    {
        var helpCommand = new HelpCommand();
        Console.WriteLine($"{HelpCommand.GeneralHelpText}\n### Commands\n{helpCommand.CommandsText.Value}");
    }
}
