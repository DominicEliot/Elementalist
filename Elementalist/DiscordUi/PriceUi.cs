using System.Text.Json;
using NetCord;
using MediatR;
using Elementalist.Features.Card;
using Elementalist.Features.Cards;
using Elementalist.Infrastructure.DataAccess.CardData;

namespace Elementalist.DiscordUi;

public class PriceUi(IMediator mediator, ICardRepository cardRepository) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IMediator _mediator = mediator;
    private readonly ICardRepository _cardRepository = cardRepository;

    [ComponentInteraction("price")]
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

            var priceQuery = new Prices.CardPriceQuery(card.Name, set.Name, variant.Finish);
            var priceResponse = await _mediator.Send(priceQuery);

            if (priceResponse.IsValid())
            {
                if (priceResponse.Value.Count() == 0)
                {
                    await RespondAsync($"No prices for {uniqueCardId}", ephemeral: true);
                    return;
                }

                var embed = CardPriceEmbed(cardName, priceResponse.Value);

                await RespondAsync(embed: embed.Build());
                return;
            }
        }

        await RespondAsync($"Couldn't find a price for {cardName}", ephemeral: true);
    }

    [SlashCommand("price", "Shows the price of the specified card.")]
    public async Task CardPriceByName([SlashCommandParameter(AutocompleteProviderType = typeof(CardAutoCompleteHandler))] string cardName, bool ephemeral = false)
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
            .WithTitle($"{cardName} Prices");

        foreach (var item in prices)
        {
            builder.AddField(item.Card.ToNamelessString(), $"{item.Condition} Low: {item.Low:C2}, Mid: {item.Mid:C2}".Trim());
        }

        return builder;
    }
}
