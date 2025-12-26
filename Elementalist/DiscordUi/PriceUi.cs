using System.Text.Json;
using Elementalist.Features.Cards;
using Elementalist.Infrastructure.DataAccess.CardData;
using MediatR;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class PriceUiSelect(IMediator mediator, ICardRepository cardRepository) : ComponentInteractionModule<ButtonInteractionContext>
{
    private readonly IMediator _mediator = mediator;
    private readonly ICardRepository _cardRepository = cardRepository;

    [ComponentInteraction("price")]
    public async Task ShowPrice(string cardName)
    {
        var message = Context.Interaction.Message;
        var components = message.Components;

        var menu = components.OfType<StringMenu>().First();
        var uniqueCardJson = menu.Options.First(o => o.Default).Value;
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
                if (!priceResponse.Value.Any())
                {
                    await RespondAsync(InteractionCallback.Message(new() { Content = $"No prices for {uniqueCardId}", Flags = MessageFlags.Ephemeral }));
                    return;
                }

                var embed = PriceUi.CardPriceEmbed(cardName, priceResponse.Value);

                await RespondAsync(InteractionCallback.Message(new() { Embeds = [embed] }));
                return;
            }
        }

        await RespondAsync(InteractionCallback.Message(new() { Content = $"Couldn't find a price for {cardName}", Flags = MessageFlags.Ephemeral }));
    }
}

public class PriceUiSlashCommand(IMediator mediator) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IMediator _mediator = mediator;

    [SlashCommand("price", "Shows the price of the specified card.")]
    public async Task CardPriceByName([SlashCommandParameter(AutocompleteProviderType = typeof(CardAutoCompleteHandler))] string cardName, bool ephemeral = false)
    {
        var priceQuery = new Prices.CardPriceQuery(cardName);
        var priceResponse = await _mediator.Send(priceQuery);

        if (priceResponse.IsError())
        {
            await RespondAsync(InteractionCallback.Message(new() { Content = $"Couldn't find prices for card {cardName}", Flags = MessageFlags.Ephemeral }));
        }

        var embed = PriceUi.CardPriceEmbed(cardName, priceResponse.Value);
        await RespondAsync(InteractionCallback.Message(new() { Embeds = [embed], Flags = ephemeral ? MessageFlags.Ephemeral : null }));
    }
}

internal static class PriceUi
{
    internal static EmbedProperties CardPriceEmbed(string cardName, IEnumerable<Prices.PriceData> prices)
    {
        var builder = new EmbedProperties()
            .WithTitle($"{cardName} Prices");

        var fields = new List<EmbedFieldProperties>();
        foreach (var item in prices)
        {
            fields.Add(new EmbedFieldProperties()
                .WithName(item.Card.ToNamelessString())
                .WithValue($"{item.Condition} Low: {item.Low:C2}, Mid: {item.Mid:C2}".Trim())
                );
        }
        builder.AddFields(fields);

        return builder;
    }
}
