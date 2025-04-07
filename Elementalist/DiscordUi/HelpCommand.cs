using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Elementalist.DiscordUi;
using Elementalist.Infrastructure.DataAccess.CardData;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace ElementalistBot.DiscordUi;

public class HelpCommand : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("help", "Gives descriptions for all the bot commands, and some information about the bot.")]
    public string ShowHelpText()
    {
        return ($"{GeneralHelpText}\n### Commands\n{CommandsText.Value}");
    }

    public static Lazy<string> CommandsText = new Lazy<string>(() =>
    {
        var slashCommands = Assembly.GetExecutingAssembly().GetTypes()
            //Todo: get real interface
            .Where(t => t.IsAssignableTo(typeof(ApplicationCommandModule<>)))
            .SelectMany(T => T.GetMembers())
            .Select(M => Attribute.GetCustomAttribute(M, typeof(SlashCommandAttribute)))
            .OfType<SlashCommandAttribute>()
            .Where(a => a is not null);

        return string.Join("\n", slashCommands.Select(c => $"* `{c.Name}` - {c.Description}"));
    });

    public const string GeneralHelpText =
        @"### General
        1. To add the bot to a server click the bot's portrait icon, then click the `Add App` button.
        1. To report a bug please fill out a issue on the bot's [github page](https://github.com/XenotropicDev/Elementalist)
        ";
}
