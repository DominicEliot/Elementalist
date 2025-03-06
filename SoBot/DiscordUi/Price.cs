using System.Text.Json;
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
        var uniqueCardJson = selectedMenus.First().Options.First(o => o.IsDefault == true).Value;
        var uniqueCardId = JsonSerializer.Deserialize<UniqueCardIdentifier>(uniqueCardJson);

        var card = (await _cardRepository.GetCardsMatching(c => c.Name == cardName)).FirstOrDefault();

        if (card is not null && uniqueCardId is not null)
        {
            var set = card.Sets.First(s => s.Name == uniqueCardId.Set);
            var variant = set.Variants.First(v => v.Finish == uniqueCardId.Finish && v.Product == uniqueCardId.Product);

            //Tcgplayer uses different finish names than Curiosa.io
            var finish = variant.Finish == "Standard" ? "Normal" : variant.Finish;

            var priceQuery = new Prices.CardPriceQuery(card.Name, set.Name, finish);
            var priceResponse = await _mediator.Send(priceQuery);

            if (priceResponse.IsValid())
            {
                var embed = CardPriceEmbed(cardName, priceResponse.Value);

                await RespondAsync(embed: embed.Build());
                return;
            }
        }

        await RespondAsync($"Couldn't find a price for {cardName}");
    }

    [SlashCommand("price", "Shows the price of the specified card.")]
    public async Task CardPriceByName([Autocomplete<CardAutoCompleteHandler>()] string cardName, bool ephemeral = false)
    {
        var cards = await _mediator.Send(new GetCardsQuery() { CardNameContains = cardName });

        var priceQuery = new Prices.CardPriceQuery(cardName);
        var priceResponse = await _mediator.Send(priceQuery);

        if (priceResponse.IsError())
        {
            await RespondAsync($"Couldn't find prices for card {cardName}", ephemeral: true);
        }

        var embed = CardPriceEmbed(cardName, priceResponse.Value);
        await RespondAsync(embed: embed.Build());
    }

    private static EmbedBuilder CardPriceEmbed(string cardName, IEnumerable<Prices.PriceData> prices)
    {
        var builder = new EmbedBuilder()
            .WithTitle($"{cardName} Prices")
            .WithCurrentTimestamp();

        foreach (var item in prices)
        {
            builder.AddField(item.Card.ToNamelessString(), $"Low: {item.Low:C2}, Mid: {item.Mid:C2}");
        }

        return builder;
    }
}
