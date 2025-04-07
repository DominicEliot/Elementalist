using System.Text.Json;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Models;
using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class CardArtUi(ICardRepository cardRepository) : ComponentInteractionModule<ButtonInteractionContext>
{
    private readonly ICardRepository _cardRepo = cardRepository;

    [ComponentInteraction("art")]
    public async Task ShowFaq(string cardName)
    {
        var buttonMessage = Context.Message;
        var components = buttonMessage.Components;

        var selectedMenus = components.OfType<StringMenu>(); //?.Options.FirstOrDefault(o => o.IsDefault == true)?.Value;
        var uniqueCardJson = selectedMenus.First().Options.First(o => o.Default == true).Value;
        var cardVersionMetadata = JsonSerializer.Deserialize<UniqueCardIdentifier>(uniqueCardJson);

        var message = new InteractionMessageProperties();

        if (cardVersionMetadata is null)
        {
            message.WithContent($"Couldn't load art for {uniqueCardJson}").WithFlags(MessageFlags.Ephemeral);
            await RespondAsync(InteractionCallback.Message(message));
            return;
        }

        var card = (await _cardRepo.GetCardsMatching(c => c.Name == cardName)).Single();

        var set = card.Sets.FirstOrDefault(s => s.Name == cardVersionMetadata.Set);
        var variant = set?.Variants.FirstOrDefault(v => v.Product == cardVersionMetadata.Product && v.Finish == cardVersionMetadata.Finish);

        if (set is null || variant is null)
        {
            message.WithContent($"Couldn't load art for {uniqueCardJson}").WithFlags(MessageFlags.Ephemeral);
            await RespondAsync(InteractionCallback.Message(message));
            return;
        }

        var setVariant = new SetVariant() { Set = set, Variant = variant };

        var cardArtEmbed = new EmbedCardArtAdapter(card, setVariant);
        message.Embeds = [cardArtEmbed];

        await RespondAsync(InteractionCallback.Message(message));
    }

    internal class EmbedCardArtAdapter : EmbedProperties
    {
        public EmbedCardArtAdapter(Models.Card card, SetVariant? setVariant = null)
        {
            setVariant ??= CardLookups.GetDefaultVariant(card);

            //sample style: https://message.style/app/editor/share/KYfJ50a5
            WithAuthor(new() { Name = card.Name });
            WithColor(DiscordHelpers.GetCardColor(card.Elements));
            WithImage(new(CardArt.GetUrl(setVariant)));
            WithFooter(new() { Text = $"Art @ {setVariant.Variant.Artist}" });
        }
    }
}
