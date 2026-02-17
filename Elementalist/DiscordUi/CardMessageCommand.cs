using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Elementalist.DiscordUi;
using Elementalist.Features.Cards;
using Elementalist.Models;
using MediatR;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace ElementalistBot.DiscordUi;

public partial class CardMessageCommand(IMediator mediator, CardDisplayService cardDisplayService) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IMediator _mediator = mediator;
    private readonly CardDisplayService _cardDisplayService = cardDisplayService;

    [MessageCommand("show-card-info")]
    public async Task ShowCardsFromMessageCommand(RestMessage message)
    {
        var cardsToShow = await FindCardsInText(message.Content);
        if (!cardsToShow.Any())
        {
            await RespondAsync(InteractionCallback.Message(new() { Content = $"No cards found in text. Make sure they are either [bracketed] or in (parenthesis).", Flags = NetCord.MessageFlags.Ephemeral } ));
            return;
        }

        var responseMessage = await _cardDisplayService.CardInfoMessage(cardsToShow, null, Context.Guild?.Id ?? 0);

        await RespondAsync(InteractionCallback.Message(responseMessage));
    }

    private async Task<IEnumerable<Card>> FindCardsInText(string textToSearch)
    {
        var cardsToShow = new List<Card>();
        foreach (Match match in CardNameInTextRegex().Matches(textToSearch))
        {
            var cardName = match.Groups[1].Value;

            var query = new GetCardsQuery() { CardNameContains = cardName };
            var cards = await _mediator.Send(query);

            cardsToShow.AddRange(cards);
        }

        return cardsToShow;
    }

    [GeneratedRegex(@"[([]+(.*?)[)\]]+")]
    private static partial Regex CardNameInTextRegex();
}
