using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoBot.UI.SlashCommands;
public class CardSearch : InteractionModuleBase
{
    [SlashCommand("card", "Searches for and returns any matching sorcery cards")]
    public async Task CardSlashCommand(string cardName)
    {
        await RespondAsync($"{cardName}'s info should go here.");
    }
}
