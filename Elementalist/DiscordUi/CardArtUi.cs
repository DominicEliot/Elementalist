using System.Text.Json;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Models;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class CardArtUi(ICardRepository cardRepository, CardArtService cardArtService) : ComponentInteractionModule<ButtonInteractionContext>
{
    private readonly ICardRepository _cardRepo = cardRepository;

    [ComponentInteraction("art")]
    public async Task ShowCardArt(string cardName)
    {
        var buttonMessage = Context.Message;
        var components = buttonMessage.Components;

        var selectedMenus = components.OfType<StringMenu>();
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

        var printing = card.Printings.FirstOrDefault(p =>
            p.Set.Name == cardVersionMetadata.Set
            && p.Meta.Product == cardVersionMetadata.Product
            && p.Meta.Finish.ToString() ==  cardVersionMetadata.Finish);

        if (printing is null)
        {
            message.WithContent($"Couldn't load art for {uniqueCardJson}").WithFlags(MessageFlags.Ephemeral);
            await RespondAsync(InteractionCallback.Message(message));
            return;
        }

        var cardArtEmbed = new EmbedCardArtAdapter(card, cardArtService, printing);
        message.Embeds = [cardArtEmbed];

        await RespondAsync(InteractionCallback.Message(message));
    }

    internal class EmbedCardArtAdapter : EmbedProperties
    {
        public EmbedCardArtAdapter(Card card, CardArtService cardArtService, CardPrinting? setVariant = null)
        {
            setVariant ??= CardLookups.GetDefaultVariant(card);

            //sample style: https://message.style/app/editor/share/KYfJ50a5
            WithAuthor(new() { Name = card.Name });
            WithColor(DiscordHelpers.GetCardColor(card.Engine.Elements));
            WithImage(new(cardArtService.GetUrl(setVariant)));
            WithFooter(new() { Text = $"Art @ {setVariant.Meta.Artist.Name}" });
        }
    }
}
