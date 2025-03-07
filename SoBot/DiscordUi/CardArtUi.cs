using System.Text.Json;
using Discord;
using Discord.Interactions;
using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBot.DiscordUi;

public class CardArtUi(ICardRepository cardRepository) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICardRepository _cardRepo = cardRepository;

    [ComponentInteraction("art-*")]
    public async Task ShowFaq(string cardName)
    {
        if (!(Context.Interaction is IComponentInteraction { } interaction)) throw new ArgumentNullException(nameof(Context.Interaction));
        var message = interaction.Message;
        var components = message.Components;

        var selectedMenus = components.OfType<ActionRowComponent>().SelectMany(ar => ar.Components.OfType<SelectMenuComponent>()); //?.Options.FirstOrDefault(o => o.IsDefault == true)?.Value;
        var uniqueCardJson = selectedMenus.First().Options.First(o => o.IsDefault == true).Value;
        var cardVersionMetadata = JsonSerializer.Deserialize<UniqueCardIdentifier>(uniqueCardJson);

        if (cardVersionMetadata is null)
        {
            await RespondAsync($"Couldn't load art for {uniqueCardJson}", ephemeral: true);
            return;
        }

        var card = (await _cardRepo.GetCardsMatching(c => c.Name == cardName)).Single();

        var set = card.Sets.FirstOrDefault(s => s.Name == cardVersionMetadata.Set);
        var variant = set?.Variants.FirstOrDefault(v => v.Product == cardVersionMetadata.Product && v.Finish == cardVersionMetadata.Finish);

        if (set is null || variant is null)
        {
            await RespondAsync($"Couldn't load art for {uniqueCardJson}", ephemeral: true);
            return;
        }

        var setVariant = new SetVariant() { Set = set, Variant = variant };

        var embedBuilder = new EmbedCardArtAdapter(card, setVariant);
    }

    internal class EmbedCardArtAdapter : EmbedBuilder
    {
        public EmbedCardArtAdapter(Models.Card card, SetVariant? setVariant = null)
        {
            setVariant ??= CardLookups.GetDefaultVariant(card);

            //sample style: https://message.style/app/editor/share/KYfJ50a5
            WithAuthor(card.Name);
            WithColor(DiscordHelpers.GetCardColor(card.Elements));
            WithThumbnailUrl(CardArt.GetUrl(setVariant));
            WithFooter($"Art @ {setVariant.Variant.Artist}");
        }
    }
}
