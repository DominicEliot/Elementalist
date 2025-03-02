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
    private readonly ICardRepository _cardRepository = cardRepository;

    [ComponentInteraction("price-*")]
    public async Task ShowPrice(string cardName)
    {
        if (!(Context.Interaction is IComponentInteraction { } interaction)) throw new ArgumentNullException(nameof(Context.Interaction));
        var message = interaction.Message;
        var components = message.Components;

        var selectedMenus = components.OfType<ActionRowComponent>().SelectMany(ar => ar.Components.OfType<SelectMenuComponent>()); //?.Options.FirstOrDefault(o => o.IsDefault == true)?.Value;
        var selectedSlug = selectedMenus.FirstOrDefault()?.Options.FirstOrDefault(o => o.IsDefault == true)?.Value;

        var card = (await _cardRepository.GetCardsMatching(c => c.Name == cardName)).FirstOrDefault();

        if (card != null)
        {
            var set = card.Sets.First(s => s.Variants.Any(v => v.Slug == selectedSlug));
            var variant = set.Variants.First(v => v.Slug == selectedSlug);
            var finish = variant.Finish == "Standard" ? "Normal" : variant.Finish;

            var priceQuery = new Prices.CardPriceQuery(card.Name, set.Name, finish);
            var priceResponse = await _mediator.Send(priceQuery);

            if (priceResponse.IsValid())
            {
                await RespondAsync($"Price for {selectedSlug}: {priceResponse.Value.Mid}");
                return;
            }
        }

        await RespondAsync($"Couldn't find a price for {cardName}");
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
