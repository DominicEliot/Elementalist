using Discord;
using Discord.Interactions;
using MediatR;
using SorceryBot.Features.Card;
using SorceryBot.Features.Cards;
using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBot.DiscordUi;

public class PriceUi(IMediator mediator, ICardRepository cardRepository) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator = mediator;

    [ComponentInteraction("price-*")]
    public async Task ShowPrice(string cardSlug)
    {
        var priceQuery = new Prices.CardPriceQuery(cardSlug);
    }

    [SlashCommand("price", "Shows the price of the specified card.")]
    public async Task CardPriceByName([Autocomplete<CardAutoCompleteHandler>()] string cardName, bool ephemeral = false)
    {
        var cards = await _mediator.Send(new GetCardsQuery() { CardNameContains = cardName });

        var builder = new EmbedBuilder()
            .WithTitle($"{cardName} Price")
            .AddField("Mid", "$ TODO", inline: true)
            .AddField("Low", "$ TODO", inline: true);

        await RespondAsync(embed: builder.Build());
    }
}
