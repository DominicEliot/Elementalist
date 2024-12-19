using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using MediatR;
using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBot.Features.Card;
public static class Faq
{
    public class CardFaqSlashCommand(IMediator mediator) : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator = mediator;

        [SlashCommand("search", "Searches for and returns any matching sorcery cards")]
        public async Task FaqCommand(string cardName)
        {
            var cards = await _mediator.Send(new GetCardsQuery(cardName));
            var card = cards.FirstOrDefault();

            await RespondAsync($"{cardName}'s info should go here.");
        }
    }
}
