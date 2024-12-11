using Discord.Interactions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SorceryBot.Features.Card.Search;
public class CardSearchSlashCommand : InteractionModuleBase
{
    private readonly IMediator _mediator;

    public CardSearchSlashCommand(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SlashCommand("card", "Searches for and returns any matching sorcery cards")]
    public async Task CardSearch(string cardName)
    {
        var card = await _mediator.Send(new CardSearchQuery());
        await RespondAsync($"{cardName}'s info should go here.");
    }
}
