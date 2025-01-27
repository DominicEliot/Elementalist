using Discord;
using Discord.Interactions;
using MediatR;
using SorceryBot.Features.Card;

namespace SorceryBot.DiscordUi;

internal class PriceUi(IMediator mediator) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator = mediator;

    [ComponentInteraction("price-{card}")]
    public async Task ShowPrice(string card)
    {
    }

    [SlashCommand("price", "Shows the price of the specified card.")]
    public async Task CardSearchByName([Autocomplete<CardAutoCompleteHandler>()] string cardName, bool ephemeral = false)
    {
        var cards = await _mediator.Send(new GetCardsQuery() { CardNameContains = cardName });

        var builder = new EmbedBuilder()
            .WithTitle($"{cardName} Price")
            .w
            .AddField("Mid", "$ TODO", inline: true)
            .AddField("Low", "$ TODO", inline: true);

        await RespondAsync(embed: builder.Build());
    }
}
