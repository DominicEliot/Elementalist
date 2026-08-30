using System.Text.Json;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Models;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class VariantSelect(ICardRepository cardRepository, CardDisplayService cardDisplayService) : ComponentInteractionModule<StringMenuInteractionContext>
{
    private readonly ICardRepository _cardRepository = cardRepository;
    private readonly CardDisplayService _cardDisplayService = cardDisplayService;

    [ComponentInteraction("variantSelect")]
    public async Task SelectVariant()
    {
        var uniqueCard = JsonSerializer.Deserialize<UniqueCardIdentifier>(Context.SelectedValues[0]);

        if (uniqueCard == null)
        {
            await RespondAsync(InteractionCallback.Message(new() { Content = "Unknown card data format", Flags = MessageFlags.Ephemeral }));
            return;
        }

        var card = (await _cardRepository.GetCardsMatching(c => c.Name == uniqueCard.Name)).First();
        var printing = card.Printings.First(p => p.Set.Name == uniqueCard.Set
            && p.Meta.Finish.ToString() == uniqueCard.Finish
            && p.Meta.Product == uniqueCard.Product);

        var message = await _cardDisplayService.CardInfoMessage([card], printing, Context.GetGuildId() ?? 0);

        var callback = InteractionCallback.ModifyMessage(m =>
        {
            m.Components = message.Components;
            m.Embeds = message.Embeds;
        });
        await RespondAsync(callback);
    }
}
